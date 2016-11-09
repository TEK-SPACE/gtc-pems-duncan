using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Duncan.PEMS.SpaceStatus.UtilityClasses
{
    public class TimeSlot
    {
        // ----------------------------------------------------------------------
        public static readonly TimeSlot Anytime = new TimeSlot(true);

        // ----------------------------------------------------------------------
        public TimeSlot() :
            this(DateTime.MinValue, DateTime.MaxValue)
        {
        } // TimeSlot

        // ----------------------------------------------------------------------
        internal TimeSlot(bool isReadOnly = false) :
            this(DateTime.MinValue, DateTime.MaxValue, isReadOnly)
        {
        } // TimeSlot

        // ----------------------------------------------------------------------
        public TimeSlot(DateTime moment, bool isReadOnly = false) :
            this(moment, TimeSpan.Zero, isReadOnly)
        {
        } // TimeSlot

        // ----------------------------------------------------------------------
        public TimeSlot(DateTime start, DateTime end, bool isReadOnly = false)
        {
            if (start <= end)
            {
                this.start = start;
                this.end = end;
            }
            else
            {
                this.end = start;
                this.start = end;
            }
            duration = this.end - this.start;
            this.isReadOnly = isReadOnly;
        } // TimeSlot

        // ----------------------------------------------------------------------
        public TimeSlot(DateTime start, TimeSpan duration, bool isReadOnly = false)
        {
            if (duration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("duration");
            }
            this.start = start;
            this.duration = duration;
            end = start.Add(duration);
            this.isReadOnly = isReadOnly;
        } // TimeSlot

        // ----------------------------------------------------------------------
        public TimeSlot(TimeSpan duration, DateTime end, bool isReadOnly = false)
        {
            if (duration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("duration");
            }
            this.end = end;
            this.duration = duration;
            start = end.Subtract(duration);
            this.isReadOnly = isReadOnly;
        } // TimeSlot

        // ----------------------------------------------------------------------
        public TimeSlot(TimeSlot copy)
        {
            if (copy == null)
            {
                throw new ArgumentNullException("copy");
            }
            start = copy.Start;
            end = copy.End;
            duration = copy.Duration;
            isReadOnly = copy.IsReadOnly;
        } // TimeSlot

        // ----------------------------------------------------------------------
        protected TimeSlot(TimeSlot copy, bool isReadOnly)
        {
            if (copy == null)
            {
                throw new ArgumentNullException("copy");
            }
            start = copy.Start;
            end = copy.End;
            duration = copy.Duration;
            this.isReadOnly = isReadOnly;
        } // TimeSlot

        // ----------------------------------------------------------------------
        public bool IsReadOnly
        {
            get { return isReadOnly; }
        } // IsReadOnly

        // ----------------------------------------------------------------------
        public bool IsAnytime
        {
            get { return !HasStart && !HasEnd; }
        } // IsAnytime

        // ----------------------------------------------------------------------
        public bool IsMoment
        {
            get { return start.Equals(end); }
        } // IsMoment

        // ----------------------------------------------------------------------
        public bool HasStart
        {
            get { return start != DateTime.MinValue; }
        } // HasStart

        // ----------------------------------------------------------------------
        public DateTime Start
        {
            get { return start; }
            set
            {
                CheckModification();
                start = value;
                end = start.Add(duration);
            }
        } // Start

        // ----------------------------------------------------------------------
        public bool HasEnd
        {
            get { return end != DateTime.MaxValue; }
        } // HasEnd

        // ----------------------------------------------------------------------
        public DateTime End
        {
            get { return end; }
            set
            {
                CheckModification();
                end = value;
                start = end.Subtract(duration);
            }
        } // End

        // ----------------------------------------------------------------------
        public TimeSpan Duration
        {
            get { return duration; }
            set { DurationFromStart(value); }
        } // Duration

        public Object Tag { get; set; }

        // ----------------------------------------------------------------------
        public virtual void Setup(DateTime newStart, DateTime newEnd)
        {
            CheckModification();
            if (newStart <= newEnd)
            {
                start = newStart;
                end = newEnd;
            }
            else
            {
                end = newStart;
                start = newEnd;
            }
            duration = end - start;
        } // Setup

        // ----------------------------------------------------------------------
        public virtual void Setup(DateTime newStart, TimeSpan newDuration)
        {
            CheckModification();
            if (newDuration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("newDuration");
            }
            start = newStart;
            duration = newDuration;
            end = start.Add(duration);
        } // Setup

        // ----------------------------------------------------------------------
        public TimeSlot Copy()
        {
            return Copy(TimeSpan.Zero);
        } // Copy

        // ----------------------------------------------------------------------
        public virtual TimeSlot Copy(TimeSpan offset)
        {
            return new TimeSlot(start.Add(offset), end.Add(offset), IsReadOnly);
        } // Copy

        // ----------------------------------------------------------------------
        public virtual void Move(TimeSpan offset)
        {
            CheckModification();
            if (offset == TimeSpan.Zero)
            {
                return;
            }
            start = start.Add(offset);
            end = end.Add(offset);
        } // Move

        // ----------------------------------------------------------------------
        public TimeSlot GetPreviousPeriod()
        {
            return GetPreviousPeriod(TimeSpan.Zero);
        } // GetPreviousPeriod

        // ----------------------------------------------------------------------
        public virtual TimeSlot GetPreviousPeriod(TimeSpan offset)
        {
            return new TimeSlot(Duration, Start.Add(offset), IsReadOnly);
        } // GetPreviousPeriod

        // ----------------------------------------------------------------------
        public TimeSlot GetNextPeriod()
        {
            return GetNextPeriod(TimeSpan.Zero);
        } // GetNextPeriod

        // ----------------------------------------------------------------------
        public virtual TimeSlot GetNextPeriod(TimeSpan offset)
        {
            return new TimeSlot(End.Add(offset), Duration, IsReadOnly);
        } // GetNextPeriod

        // ----------------------------------------------------------------------
        public virtual void DurationFromStart(TimeSpan newDuration)
        {
            if (newDuration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("newDuration");
            }
            CheckModification();

            duration = newDuration;
            end = start.Add(newDuration);
        } // DurationFromStart

        // ----------------------------------------------------------------------
        public virtual void DurationFromEnd(TimeSpan newDuration)
        {
            if (newDuration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("newDuration");
            }
            CheckModification();

            duration = newDuration;
            start = end.Subtract(newDuration);
        } // DurationFromEnd

        // ----------------------------------------------------------------------
        public virtual bool IsSamePeriod(TimeSlot test)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }
            return start == test.Start && end == test.End;
        } // IsSamePeriod


        // ----------------------------------------------------------------------
        public virtual bool HasInside(DateTime test)
        {
            return TimePeriodCalc.HasInside(this, test);
        } // HasInside

        // ----------------------------------------------------------------------
        public virtual bool HasInside(TimeSlot test)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }
            return TimePeriodCalc.HasInside(this, test);
        } // HasInside

        // ----------------------------------------------------------------------
        public virtual bool IntersectsWith(TimeSlot test)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }
            return TimePeriodCalc.IntersectsWith(this, test);
        } // IntersectsWith

        // ----------------------------------------------------------------------
        public virtual TimeSlot GetIntersection(TimeSlot period)
        {
            if (period == null)
            {
                throw new ArgumentNullException("period");
            }
            if (!IntersectsWith(period))
            {
                return null;
            }
            DateTime periodStart = period.Start;
            DateTime periodEnd = period.End;
            return new TimeSlot(
                periodStart > start ? periodStart : start,
                periodEnd < end ? periodEnd : end,
                IsReadOnly);
        } // GetIntersection

        // ----------------------------------------------------------------------
        public virtual bool OverlapsWith(TimeSlot test)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }
            return TimePeriodCalc.OverlapsWith(this, test);
        } // OverlapsWith

        // ----------------------------------------------------------------------
        public virtual PeriodRelation GetRelation(TimeSlot test)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }
            return TimePeriodCalc.GetRelation(this, test);
        } // GetRelation


        // ----------------------------------------------------------------------
        public virtual void Reset()
        {
            CheckModification();
            start = DateTime.MinValue;
            duration = DateTime.MaxValue - DateTime.MinValue;
            end = DateTime.MaxValue;
        } // Reset

        // ----------------------------------------------------------------------
        protected virtual bool IsEqual(object obj)
        {
            return HasSameData(obj as TimeSlot);
        } // IsEqual

        // ----------------------------------------------------------------------
        private bool HasSameData(TimeSlot comp)
        {
            return start == comp.start && end == comp.end && isReadOnly == comp.isReadOnly;
        } // HasSameData

        // ----------------------------------------------------------------------
        protected void CheckModification()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("TimeSlot is read-only");
            }
        } // CheckModification


        // ----------------------------------------------------------------------
        // members
        private readonly bool isReadOnly;
        private DateTime start;
        private TimeSpan duration;
        private DateTime end;  // cache    }
    }

    internal static class TimePeriodCalc
    {

        // ----------------------------------------------------------------------
        public static bool HasInside(TimeSlot period, DateTime test)
        {
            return test >= period.Start && test <= period.End;
        } // HasInside

        // ----------------------------------------------------------------------
        public static bool HasInside(TimeSlot period, TimeSlot test)
        {
            return HasInside(period, test.Start) && HasInside(period, test.End);
        } // HasInside

        // ----------------------------------------------------------------------
        public static bool IntersectsWith(TimeSlot period, TimeSlot test)
        {
            return
                HasInside(period, test.Start) ||
                HasInside(period, test.End) ||
                (test.Start < period.Start && test.End > period.End);
        } // IntersectsWith

        // ----------------------------------------------------------------------
        public static bool OverlapsWith(TimeSlot period, TimeSlot test)
        {
            PeriodRelation relation = GetRelation(period, test);
            return
                relation != PeriodRelation.After &&
                relation != PeriodRelation.StartTouching &&
                relation != PeriodRelation.EndTouching &&
                relation != PeriodRelation.Before;
        } // OverlapsWith

        // ----------------------------------------------------------------------
        public static PeriodRelation GetRelation(TimeSlot period, TimeSlot test)
        {
            if (test.End < period.Start)
            {
                return PeriodRelation.After;
            }
            if (test.Start > period.End)
            {
                return PeriodRelation.Before;
            }
            if (test.Start == period.Start && test.End == period.End)
            {
                return PeriodRelation.ExactMatch;
            }
            if (test.End == period.Start)
            {
                return PeriodRelation.StartTouching;
            }
            if (test.Start == period.End)
            {
                return PeriodRelation.EndTouching;
            }
            if (HasInside(period, test))
            {
                if (test.Start == period.Start)
                {
                    return PeriodRelation.EnclosingStartTouching;
                }
                return test.End == period.End ? PeriodRelation.EnclosingEndTouching : PeriodRelation.Enclosing;
            }
            bool periodContainsMyStart = HasInside(test, period.Start);
            bool periodContainsMyEnd = HasInside(test, period.End);
            if (periodContainsMyStart && periodContainsMyEnd)
            {
                if (test.Start == period.Start)
                {
                    return PeriodRelation.InsideStartTouching;
                }
                return test.End == period.End ? PeriodRelation.InsideEndTouching : PeriodRelation.Inside;
            }
            if (periodContainsMyStart)
            {
                return PeriodRelation.StartInside;
            }
            if (periodContainsMyEnd)
            {
                return PeriodRelation.EndInside;
            }
            throw new InvalidOperationException("invalid period relation of '" + period + "' and '" + test + "'");
        } // GetRelation

    } // class TimePeriodCalc

    public enum PeriodRelation
    {
        After,
        StartTouching,
        StartInside,
        InsideStartTouching,
        EnclosingStartTouching,
        Enclosing,
        EnclosingEndTouching,
        ExactMatch,
        Inside,
        InsideEndTouching,
        EndInside,
        EndTouching,
        Before,
    } // enum PeriodRelation

    public class TimeSlotGapCalculator
    {
        public static List<TimeSlot> GetGaps(TimeSlot overallRange, List<TimeSlot> comparisonSlots)
        {
            List<TimeSlot> result = new List<TimeSlot>();

            TimeSlot remainingRange = new TimeSlot(overallRange);
            int recursions = 0;
            while (remainingRange.Start < overallRange.End)
            {
                recursions++;
                TimeSlot nextComparison = null;
                foreach (TimeSlot nextSlot in comparisonSlots)
                {
                    // Does the next slot have a portion within the remaining range?
                    if (nextSlot.IntersectsWith(remainingRange))
                    {
                        TimeSlot intersection = nextSlot.GetIntersection(remainingRange);
                        if (nextComparison == null)
                        {
                            nextComparison = intersection;
                        }
                        else
                        {
                            // Does the next slot have a start or end that is sooner than the comparison object we already selected?
                            if ((intersection.Start < nextComparison.Start) || (intersection.End < nextComparison.Start))
                            {
                                nextComparison = intersection;
                            }
                        }
                    }
                }

                // If there is no comparison slot, then any remaining range is the final gap
                if (nextComparison == null)
                {
                    result.Add(remainingRange);
                    break;
                }

                // Create the gap and add to list (unless its duration is zero)
                TimeSlot gap = new TimeSlot(remainingRange.Start, nextComparison.Start);
                if (gap.Duration.TotalSeconds >= 1)
                    result.Add(gap);

                // Adjust remaining range to check
                remainingRange = new TimeSlot(nextComparison.End.AddSeconds(1), overallRange.End);
            }

            /*
            if ((recursions > 10) || (result.Count > 10))
                System.Diagnostics.Debug.WriteLine("TimeSlotGapCalculator.GetGaps took " + recursions.ToString() + " recursions");
            */

            return result;
        }
    }
}