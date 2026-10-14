using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;

namespace JHSchool
{
    /// <summary>
    /// 一次預先載入多個使用者組態。
    /// </summary>
    /// <remarks>
    /// Framework 的 User.Configuration["X"] 遇到未快取的組態時，會在呼叫端執行緒同步發出一次查詢，
    /// 而且多次讀取會被快取的鎖序列化。啟動時在 UI 執行緒逐一讀取 6 個組態，就是 6 次往返。
    /// 這裡透過反射呼叫內部快取的 SyncData(IEnumerable&lt;string&gt;)，以一次查詢取回全部組態，
    /// 之後的讀取直接命中快取。反射失敗時不做任何事，後續讀取會照原本方式逐一查詢。
    /// </remarks>
    internal static class UserConfigPrefetch
    {
        public static void Prefetch(params string[] configNamespaces)
        {
            try
            {
                ConfigurationManager manager = User.Configuration;
                if (manager == null) return;

                MethodInfo preprocess = typeof(ConfigurationManager).GetMethod("NamespacePreprocess", BindingFlags.Instance | BindingFlags.NonPublic);
                PropertyInfo cacheProperty = typeof(ConfigurationManager).GetProperty("Cache", BindingFlags.Instance | BindingFlags.NonPublic);
                if (preprocess == null || cacheProperty == null)
                    return;

                object cache = cacheProperty.GetValue(manager, null);
                MethodInfo syncData = cache.GetType().GetMethod("SyncData", new Type[] { typeof(IEnumerable<string>) });
                if (syncData == null)
                    return;

                List<string> keys = new List<string>();
                foreach (string configNamespace in configNamespaces)
                    keys.Add((string)preprocess.Invoke(manager, new object[] { configNamespace }));

                syncData.Invoke(cache, new object[] { keys });

                // 伺服器上不存在的組態不會進快取，之後每次讀取都會再查詢一次。
                // 背景寫入空白組態（等同全部使用預設值，行為不變），下次啟動起即可一併預先載入。
                // 注意：不可用 manager[...] 取得組態再 Save：遇到不存在的組態時，索引子會在 UI 執行緒
                // 同步查詢一次，而 Save 又會鎖住組態快取、擋住其他組態讀取。這裡直接以 Provider 批次寫入，不經過快取。
                object items = cache.GetType().GetProperty("Items").GetValue(cache, null);
                MethodInfo containsKey = items.GetType().GetMethod("ContainsKey");
                List<ConfigurationRecord> missingRecords = new List<ConfigurationRecord>();
                foreach (string key in keys)
                {
                    if (!(bool)containsKey.Invoke(items, new object[] { key }))
                        missingRecords.Add(new ConfigurationRecord(key));
                }

                PropertyInfo providerProperty = typeof(ConfigurationManager).GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
                if (missingRecords.Count > 0 && providerProperty != null)
                {
                    IConfigurationProvider provider = (IConfigurationProvider)providerProperty.GetValue(manager, null);
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            provider.SaveConfiguration(missingRecords);
                        }
                        catch
                        {
                            // 建立失敗只影響下次啟動的預先載入，讀取時仍會逐一查詢
                        }
                    });
                }
            }
            catch
            {
                // 反射失敗時不做任何事，後續讀取照原本方式逐一查詢
            }
        }
    }
}
