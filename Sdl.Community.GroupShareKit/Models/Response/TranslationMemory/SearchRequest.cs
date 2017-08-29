﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.GroupShareKit.Models.Response.TranslationMemory
{
    public class SearchRequest
    {
        public string SourceLanguageCode { get; set; }
        public string TargetLanguageCode { get; set; }
        public Guid TmId { get; set; }
        public string SearchText { get; set; }

        ///mai trebuie pusa proprietatea pt custom filter expression
        ///
        public SearchRequest(Guid tmId, string searchText,string sourceCode,string targetCode )
        {
            SourceLanguageCode = sourceCode;
            TargetLanguageCode = targetCode;
            TmId = tmId;
            SearchText = searchText;

        }
    }
}