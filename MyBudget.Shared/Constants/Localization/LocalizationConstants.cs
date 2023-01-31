﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Shared.Constants.Localization
{
    public static class LocalizationConstants
    {
        public static readonly LanguageCode[] SupportedLanguages = {
            new LanguageCode
            {
                Code = "en-US",
                DisplayName= "English"
            },
            new LanguageCode
            {
                Code="ta",
                DisplayName="தமிழ்"
            },

        };
    }
}
