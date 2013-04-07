using Dulcet.Twitter;
using System.Linq;

namespace Inscribe.Filter.Filters.Text
{
    public class FilterViaStar : FilterVia
    {
        private FilterViaStar() : base(null) { }

        public FilterViaStar(string needle) : base(needle, false) { }

        public FilterViaStar(string needle, bool isCaseSensitive) : base(needle, isCaseSensitive) { }

        public override string Identifier
        {
            get { return base.Identifier + "*"; }
        }

        public override System.Collections.Generic.IEnumerable<string> Aliases
        {
            get { return base.Aliases.Select(s => s + "*"); }
        }

        public override string Description
        {
            get { return base.Description + "*"; }
        }

        public override string FilterStateString
        {
            get { return base.FilterStateString + "*"; }
        }

        protected override bool FilterStatus(TwitterStatusBase status)
        {
            var st = status as TwitterStatus;
            return st != null && this.Match(
                st.RetweetedOriginal != null ? st.RetweetedOriginal.Source : st.Source,
                this.needle, this.isCaseSensitive);
        }
    }
}
