﻿using System;
using System.Text.RegularExpressions;
using Inscribe.Storage;

namespace Inscribe.Configuration.Settings
{
    public class InputExperienceProperty
    {
        public InputExperienceProperty()
        {
            OfficialRetweetInReplyToRetweeter = false;
            UseInputSuggesting = true;
            SuspendAutoBindInReply = true;
            UseActiveFallback = false;
            OfficialRetweetFallback = false;
            FallbackBackTracking = true;
            IsEnabledTemporarilyUserSelection = false;
            TrimExceedChars = false;
            AutoRetryOnError = true;
            AutoRetryMaxCount = 5;
            ShowInputBlockInBottom = false;
            AutoUniquify = false;
            EnableFavoriteFallback = false;
        }

        public bool OfficialRetweetInReplyToRetweeter { get; set; }

        public bool UseInputSuggesting { get; set; }

        private HashtagAutoBindDescription[] _hashtagAutoBindDescriptions = null;
        public HashtagAutoBindDescription[] HashtagAutoBindDescriptions
        {
            get { return _hashtagAutoBindDescriptions ?? new HashtagAutoBindDescription[0]; }
            set { _hashtagAutoBindDescriptions = value; }
        }

        public bool SuspendAutoBindInReply { get; set; }

        public bool UseActiveFallback { get; set; }

        public bool FallbackBackTracking { get; set; }

        public bool OfficialRetweetFallback { get; set; }

        public bool IsEnabledTemporarilyUserSelection { get; set; }

        public bool TrimExceedChars { get; set; }

        public bool AutoRetryOnError { get; set; }

        public int AutoRetryMaxCount { get; set; }

        public bool ShowInputBlockInBottom { get; set; }

        public bool AutoUniquify { get; set; }

        public bool EnableFavoriteFallback { get; set; }

        public bool UseDpMatching { get; set; }
    }

    public class HashtagAutoBindDescription
    {

        public AutoBindStrategy Strategy { get; set; }

        public string ConditionText { get; set; }

        public bool IsNegateCondition { get; set; }

        public string TagText { get; set; }

        private bool _warnedRegexError = false;
        public bool CheckCondition(string text)
        {
            switch (Strategy)
            {
                case AutoBindStrategy.Contains:
                    return text.IndexOf(ConditionText, StringComparison.CurrentCultureIgnoreCase) >= 0 == !IsNegateCondition;
                case AutoBindStrategy.StartsWith:
                    return text.StartsWith(ConditionText, StringComparison.CurrentCultureIgnoreCase) == !IsNegateCondition;
                case AutoBindStrategy.EndsWith:
                    return text.EndsWith(ConditionText, StringComparison.CurrentCultureIgnoreCase) == !IsNegateCondition;
                case AutoBindStrategy.Regex:
                    try
                    {
                        return Regex.IsMatch(text, ConditionText) == !IsNegateCondition;
                    }
                    catch(Exception e)
                    {
                        if (!_warnedRegexError)
                        {
                            ExceptionStorage.Register(e, ExceptionCategory.UserError, "ハッシュタグ自動バインドで使われている正規表現 \"" + ConditionText + "\" に問題があります。");
                            _warnedRegexError = true;
                        }
                        return false;
                    }
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return "#" + TagText + " : " + (IsNegateCondition ? "!" : "") +
                "\"" + ConditionText + "\" - " + Strategy.ToString();
        }
    }

    public enum AutoBindStrategy
    {
        Contains = 0,
        StartsWith = 1,
        EndsWith = 2,
        Regex = 3
    }
}
