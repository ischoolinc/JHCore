using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace JHSchool.Legacy.ImportSupport
{
    public class CommentItem
    {
        protected string comment;
        public string Comment
        {
            get { return comment; }
        }
    }

    public class CorrectComment : CommentItem
    {
        public CorrectComment(string originValue, string newValue)
        {
            _origin_value = originValue;
            _new_value = newValue;

            comment = string.Format("值由「{0}」改為「{1}」。", originValue, newValue);
        }

        private string _origin_value;
        public string OriginValue
        {
            get { return _origin_value; }
        }

        private string _new_value;
        public string NewValue
        {
            get { return _new_value; }
        }
    }

    public class ErrorComment : CommentItem
    {
        public ErrorComment(string msg)
        {
            comment = msg;
        }
    }

    public class WarningComment : CommentItem
    {
        public WarningComment(string msg)
        {
            comment = msg;
        }
    }

    public class CellComment
    {
        private List<CommentItem> _comments;

        public CellComment(int rowIndex, int columnIndex)
        {
            _row_index = rowIndex;
            _column_index = columnIndex;
            _comments = new List<CommentItem>();
        }

        private int _row_index;
        public int RowIndex
        {
            get { return _row_index; }
        }

        private int _column_index;
        public int ColumnIndex
        {
            get { return _column_index; }
        }

        public CommentItem BestComment
        {
            get
            {
                foreach (CommentItem each in _comments)
                    if (each is CorrectComment) return each;

                foreach (CommentItem each in _comments)
                    if (each is ErrorComment) return each;

                foreach (CommentItem each in _comments)
                    if (each is WarningComment) return each;

                return null;
            }
        }

        public void WriteError(string msg)
        {
            _comments.Add(new ErrorComment(msg));
        }

        public void WriteWarning(string msg)
        {
            _comments.Add(new WarningComment(msg));
        }

        public void WriteCorrect(string oldValue, string newValue)
        {
            _comments.Add(new CorrectComment(oldValue, newValue));
        }

        public void WriteComment(CommentItem comment)
        {
            _comments.Add(comment);
        }

        /// <summary>
        /// 與特定的 Comment  合併。
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public void MergeFrom(CellComment comment)
        {
            _comments.AddRange(comment._comments);
        }
    }

    public class CellCommentManager : IEnumerable<CellComment>
    {
        private Dictionary<Point, CellComment> _comments;

        public CellCommentManager()
        {
            _comments = new Dictionary<Point, CellComment>();
        }

        public CellComment GetComment(int rowIndex, int columnIndex)
        {
            Point p = new Point(rowIndex, columnIndex);

            if (_comments.ContainsKey(p))
                return _comments[p];
            else
            {
                CellComment comment = new CellComment(rowIndex, columnIndex);
                _comments.Add(new Point(rowIndex, columnIndex), comment);

                return comment;
            }
        }

        public void WriteCorrect(int rowIndex, int columnIndex, string oldValue, string newValue)
        {
            CellComment comment = GetComment(rowIndex, columnIndex);
            comment.WriteCorrect(oldValue, newValue);
        }

        public void WriteError(int rowIndex, int columnIndex, string msg)
        {
            CellComment comment = GetComment(rowIndex, columnIndex);
            comment.WriteError(msg);
        }

        public void WriteWarning(int rowIndex, int columnIndex, string msg)
        {
            CellComment comment = GetComment(rowIndex, columnIndex);
            comment.WriteWarning(msg);
        }

        public void MergeFrom(CellCommentManager comments)
        {
            foreach (CellComment each in comments)
            {
                CellComment origin = GetComment(each.RowIndex, each.ColumnIndex);
                origin.MergeFrom(each);
            }
        }

        #region IEnumerable<CellComment> 成員

        public IEnumerator<CellComment> GetEnumerator()
        {
            return _comments.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _comments.Values.GetEnumerator();
        }

        #endregion
    }
}
