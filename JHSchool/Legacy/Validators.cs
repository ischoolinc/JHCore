using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.Legacy
{
    #region Validators

    public interface IValidator
    {
        object Argument { set; }
        DataGridViewCell ValidCell { set; }
        bool IsValid();
        string Message { get; }
    }

    public abstract class AbstractValidator : IValidator
    {
        private string _message;
        private object _argument;
        private DataGridViewCell _cell;

        #region IValidator 成員

        public object Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }

        public DataGridViewCell ValidCell
        {
            set { _cell = value; }
            get { return _cell; }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        protected virtual void OnInvalid(string errorMessage)
        {
            _message = errorMessage;
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion
    }

    public class SchoolYearValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            ValidCell.ErrorText = string.Empty;

            if (ValidCell.Value == null)
            {
                OnInvalid("不可空白");
                return false;
            }
            if (ValidCell.Value.ToString() == string.Empty)
            {
                OnInvalid("不可空白");
                return false;
            }

            int s;
            if (!int.TryParse(ValidCell.Value.ToString(), out s))
            {
                OnInvalid("必須為數字");
                return false;
            }

            if (s > Framework.Legacy.GlobalOld.SystemConfig.DefaultSchoolYear)
            {
                OnInvalid("超出目前學年度");
                return false;
            }
            return true;
        }
    }

    public class SemesterValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            if (ValidCell.Value == null)
            {
                OnInvalid("不可空白");
                return false;
            }
            if (ValidCell.Value.ToString() == string.Empty)
            {
                OnInvalid("不可空白");
                return false;
            }
            if (ValidCell.Value.ToString() != "1" && ValidCell.Value.ToString() != "2")
            {
                OnInvalid("只能填入1或2");
                return false;
            }
            return true;
        }
    }

    public class ScoreValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            if (ValidCell.Value == null)
                return true;
            if (ValidCell.Value.ToString() == string.Empty)
                return true;

            decimal d;
            if (!decimal.TryParse(ValidCell.Value.ToString(), out d))
            {
                OnInvalid("必須為數字");
                return false;
            }
            return true;
        }
    }

    public class TeacherBiasValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            if (ValidCell.Value == null)
                return true;
            if (ValidCell.Value.ToString() == string.Empty)
                return true;

            decimal d;
            if (!decimal.TryParse(ValidCell.Value.ToString(), out d))
            {
                OnInvalid("必須為數字");
                return false;
            }

            decimal limit = decimal.Parse(Argument.ToString());
            if (Math.Abs(d) > limit)
            {
                OnInvalid("超出限制範圍");
                return false;
            }
            return true;
        }
    }

    public class CommentValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            Dictionary<string, string> list = (Dictionary<string, string>)Argument;
            if (ValidCell.Value == null) return true;
            if (ValidCell.Value.ToString() == string.Empty) return true;
            if (list == null) return true;

            StringBuilder sb = new StringBuilder();
            string[] vs = ValidCell.Value.ToString().Split(',');
            foreach (string v in vs)
            {
                if (list.ContainsKey(v))
                    sb.Append(list[v]);
                else
                    sb.Append(v);
                sb.Append(",");
            }
            if (sb.ToString().EndsWith(","))
                sb.Remove(sb.Length - 1, 1);

            ValidCell.Value = sb.ToString();
            return true;
        }
    }

    public class AllYouWantValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            return true;
        }
    }

    public interface IColumnValidator
    {
        DataGridViewColumn ValidColumn { set; }
        object Argument { set; }
        bool IsValid();
        string Message { get; }
    }

    public abstract class AbstractColumnValidator : IColumnValidator
    {
        private DataGridViewColumn _validColumn;
        private object _argument;
        private string _message;

        protected virtual void OnInvalid(string message)
        {
            _message = message;
        }

        #region IColumnValidator 成員
        public DataGridViewColumn ValidColumn
        {
            set { _validColumn = value; }
            get { return _validColumn; }
        }

        public object Argument
        {
            set { _argument = value; }
            get { return _argument; }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public string Message
        {
            get { return _message; }
        }

        #endregion
    }

    public class NoneColumnValidator : AbstractColumnValidator
    {
    }

    public class SemesterColumnValidator : AbstractColumnValidator
    {
        public override bool IsValid()
        {
            DataGridViewColumn sy = ValidColumn;
            DataGridViewColumn sm = Argument as DataGridViewColumn;

            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (DataGridViewRow row in ValidColumn.DataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                row.ErrorText = string.Empty;
                string y = row.Cells[sy.Index].Value == null ? string.Empty : row.Cells[sy.Index].Value.ToString();
                string m = row.Cells[sm.Index].Value == null ? string.Empty : row.Cells[sm.Index].Value.ToString();
                string key = y + "-" + m;
                bool valid = true;
                if (list.ContainsKey(key))
                {
                    row.ErrorText = "學年度學期與其它資料重覆";
                    valid = false;
                }
                else
                {
                    list.Add(key, null);
                }
            }
            return true;
        }
    }

    #endregion
}