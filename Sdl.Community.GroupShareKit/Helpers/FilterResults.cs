﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using Sdl.Core.Bcm.BcmModel;
using Sdl.TmService.Sdk.Model;

namespace Sdl.Community.GroupShareKit.Helpers
{
    public static class FilterResults
    {
        public static List<FilterResponse> GetFilterResultForDocument(Document document, RestScoringResult scoringResult)
        {
            var searchResult = new List<FilterResponse>();
            if (document != null)
            {
                foreach (var file in document.Files)
                {
                    foreach (var paragraphUnit in file.ParagraphUnits)
                    {
                        if (paragraphUnit.IsStructure)
                            continue;
                        foreach (var pair in paragraphUnit.SegmentPairs)
                        {
                            var sourceText = FilterExpression.ConvertSegmentPair(pair.Source);
                            var targetText = FilterExpression.ConvertSegmentPair(pair.Target);
                            var matchScore = string.Empty;
                            if (scoringResult != null)
                            {
                                matchScore = scoringResult.Match.ToString();
                            }
                            var result = new FilterResponse
                            {
                                Source = sourceText,
                                Target = targetText,
                                MatchScore = matchScore
                            };
                            searchResult.Add(result);
                        }


                    }
                }

            }
            return searchResult;
        }
    }
}
