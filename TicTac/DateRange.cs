using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTac
{
    public class DateRange : IEquatable<DateRange>
    {

        DateTime? _startDate, _endDate;

        public DateRange() : this(new DateTime?(), new DateTime?()) { }

        public DateRange(DateTime? startDate, DateTime? endDate)
        {
            AssertStartDateFollowsEndDate(startDate, endDate);
            _startDate = startDate;
            _endDate = endDate;
        }

        //public Nullable<TimeSpan> TimeSpan {
        //    get {  return endDate – startDate; }
        //}

        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                AssertStartDateFollowsEndDate(value, _endDate);
                _startDate = value;
            }
        }

        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                AssertStartDateFollowsEndDate(_startDate, value);
                _endDate = value;
            }
        }

        private static void AssertStartDateFollowsEndDate(DateTime? startDate,
            DateTime? endDate)
        {
            if (startDate == null) throw new ArgumentNullException("startDate");
            if (endDate == null) throw new ArgumentNullException("endDate");

            if ((startDate.HasValue && endDate.HasValue) &&
                (endDate.Value < startDate.Value))
                throw new InvalidOperationException("Start Date must be less than or equal to End Date");
        }

        public DateRange GetIntersection(DateRange other)
        {
            if (!Intersects(other)) throw new InvalidOperationException("DateRanges do not intersect");
            return new DateRange(GetLaterStartDate(other.StartDate), GetEarlierEndDate(other.EndDate));
        }

        private DateTime? GetLaterStartDate(DateTime? other)
        {
            return Nullable.Compare(_startDate, other) >= 0 ? _startDate : other;
        }

        private DateTime? GetEarlierEndDate(DateTime? other)
        {
            //!endDate.HasValue == +infinity, not negative infinity
            //as is the case with !startDate.HasValue
            if (Nullable.Compare(_endDate, other) == 0) return other;
            if (_endDate.HasValue && !other.HasValue) return _endDate;
            if (!_endDate.HasValue && other.HasValue) return other;
            return (Nullable.Compare(_endDate, other) >= 0) ? other : _endDate;
        }

        public bool Intersects(DateRange other)
        {
            if ((_startDate.HasValue && other.EndDate.HasValue &&
                other.EndDate.Value < _startDate.Value) ||
                (_endDate.HasValue && other.StartDate.HasValue &&
                other.StartDate.Value > _endDate.Value) ||
                (other.StartDate.HasValue && _endDate.HasValue &&
                _endDate.Value < other.StartDate.Value) ||
                (other.EndDate.HasValue && _startDate.HasValue &&
                _startDate.Value > other.EndDate.Value))
            {
                return false;
            }
            return true;
        }

        public bool Equals(DateRange other)
        {
            if (ReferenceEquals(other, null)) return false;
            return ((_startDate == other.StartDate) && (_endDate == other.EndDate));
        }

    }


    public class DateRangeComparerByStartDate : IComparer,
    IComparer<DateRange>
    {
        public int Compare(object x, object y)
        {
            if (!(x is DateRange) || !(y is DateRange))
                throw new ArgumentException("Value not a DateRange");
            return Compare((DateRange)x, (DateRange)y);
        }
        public int Compare(DateRange x, DateRange y)
        {
            if (x.StartDate < y.StartDate) { return -1; }
            if (x.StartDate > y.StartDate) { return 1; }
            return 0;
        }
    }
}
