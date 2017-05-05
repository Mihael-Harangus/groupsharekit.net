﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Helpers;
using Sdl.Community.GroupShareKit.Models.Response;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using Xunit;
using File = System.IO.File;

namespace Sdl.Community.GroupShareKit.Tests.Integration.Clients
{
    public class TranslationMemoriesClientTest
    {
        [Fact]
        public async Task GetTms()
        {
            var groupShareClient = await Helper.GetGroupShareClient();

            var tmsResponse = await groupShareClient.TranslationMemories.GetTms();

            Assert.True(tmsResponse.Items.Count>0);
        }


        [Fact]
        public async Task GetTmById()
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var tmId = await CreateTm("NewTm");
            var tm = await groupShareClient.TranslationMemories.GetTmById(tmId);

            Assert.Equal(tm.Name, "NewTm");

        }

        [Theory]
        [InlineData("28d2843b-e038-4fa7-a5ff-bb91bd12f5a8", "2f7669f3-91a7-4c10-a689-16428b995b18")]
        public async Task GetLanguageDirectionForTm(string tmId,string languageDirectionId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var languageDirection =
                await groupShareClient.TranslationMemories.GetLanguageDirectionForTm(tmId, languageDirectionId);

            Assert.Equal(languageDirection.LanguageDirectionId,languageDirectionId);
        }

        [Theory]
        [InlineData("38d57978-0fc2-4b95-8f84-f8ca60975a10")]
        public async Task GetTmsNumberByLanguageResourceTemplateId(string id)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var tmNumber = await groupShareClient.TranslationMemories.GetTmsNumberByLanguageResourceTemplateId(id);

            Assert.True(tmNumber>0);
        }

        [Theory]
        [InlineData("e669d7d2-8ea6-4c4b-8a72-ecd7e40cf097")]
        public async Task GetTmsNumberByFieldTemplateId(string id)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var tmNumber = await groupShareClient.TranslationMemories.GetTmsNumberByFieldTemplateId(id);

            Assert.True(tmNumber > 0);
        }


        public async Task<string> CreateTm(string tmName)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var tmRequest = new CreateTmRequest
            {
                TranslationMemoryId = Guid.NewGuid().ToString(),
                Name = tmName,
                LanguageDirections = new List<LanguageDirection>
                {
                    new LanguageDirection
                    {
                        LanguageDirectionId = Guid.NewGuid().ToString(),
                        Source = "en-us",
                        Target = "de-de",
                        LastReIndexSize = null,
                        LastReIndexDate = null,
                        LastRecomputeDate = null,
                        LastRecomputeSize = null
                    }
                },
                FieldTemplateId = "ec6acfc3-e166-486f-9823-3220499dc95b",
                LanguageResourceTemplateId = "78df3807-06ac-438e-b2c8-5e233df1a6a2",
                Recognizers = "RecognizeAll",
                FuzzyIndexes = "SourceWordBased,TargetWordBased",
                Location = "/SDL Community Developers",
                WordCountFlags = "DefaultFlags",
                OwnerId = "5bdb10b8-e3a9-41ae-9e66-c154347b8d17",
                FuzzyIndexTuningSettings = new FuzzyIndexTuningSettings
                {
                    MinScoreIncrease = 20,
                    MinSearchVectorLengthSourceCharIndex = 5,
                    MinSearchVectorLengthSourceWordIndex = 3,
                    MinSearchVectorLengthTargetCharIndex = 5,
                    MinSearchVectorLengthTargetWordIndex = 3

                },            
                ContainerId = "bb9c7d71-a7b5-46ba-9f42-47ffd41b80f7"
                

            };

            var tmId = await groupShareClient.TranslationMemories.CreateTm(tmRequest);

            Assert.True(tmId!=string.Empty);
            return tmId;
        }

        
        public async Task DeleteTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            await groupShareClient.TranslationMemories.DeleteTm(tmId);
        }

        [Theory]
        [InlineData("95114373-f19a-4885-94a5-12d1a8c7ccb3")]
        public async Task UpdateTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();

            var tm = await groupShareClient.TranslationMemories.GetTmById(tmId);

            tm.Name = "Updated tm";

            await groupShareClient.TranslationMemories.Update(tmId, tm);

            var updatedTm = await groupShareClient.TranslationMemories.GetTmById(tmId);

            Assert.Equal(updatedTm.Name, "Updated tm");

        }

        [Fact]
        public async Task GetTmServiceHealth()
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var health = await groupShareClient.TranslationMemories.Health();

            Assert.Equal(health.Status,"UP");
        }

 

        [Theory]
        [InlineData("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530")]
        public async Task GetTusForTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var translationUnitRequest = new TranslationUnitDetailsRequest("de-de", "ro-ro", 0,50);

            var tus = await groupShareClient.TranslationMemories.GetTranslationUnitForTm(tmId, translationUnitRequest);

            Assert.True(tus!=null);
        }
        [Fact]
        public async Task RecomputeStatistics()
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var tmId = await CreateTm("TM");
            var request = new FuzzyRequest();

            var response = await groupShareClient.TranslationMemories.RecomputeStatistics(tmId, request);

            Assert.True(response!=null);
        }

        [Fact]
        public async Task Reindex()
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var tmId = await CreateTm("TM");
            var request = new FuzzyRequest();

            var response = await groupShareClient.TranslationMemories.Reindex(tmId, request);

            Assert.True(response != null);
        }

        [Theory]
        [InlineData("423c5f5a-e495-4bfd-9934-6f9aa40f58b8")]
        public async Task ExportTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var request = new ExportRequest
            {
                Filter = new FilterExport
                {
                    Fields = new List<FieldsDuplicate>()
                    {
                        new FieldsDuplicate
                        {
                            Type = FieldsDuplicate.TypeEnum.SingleString,
                            Name = "Added field",
                            Values = new List<string> {"andrea"}
                        }
                    },
                    Expression = "(File Format =XML)"
                }
            };
            var language = new LanguageParameters("ro-ro","en-us");

            var response = await groupShareClient.TranslationMemories.ExportTm(tmId, request, language);
            Assert.Equal(response.TranslationMemoryId,tmId);
         

        }

        [Theory]
        [InlineData("423c5f5a-e495-4bfd-9934-6f9aa40f58b8")]
        public async Task ImportTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var language = new LanguageParameters("ro-ro", "en-us");

            var rawData =
            File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\RO-EN.tmx"));

            var response = await groupShareClient.TranslationMemories.ImportTm(tmId, language,rawData, "RO-EN.tmx");

            Assert.True(response!=null);
        }



        [Theory]
        [InlineData("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530")]
        public async Task GetTusNumberForTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var languageParameters = new LanguageParameters("de-de", "ro-ro");

            var tusNumber = await groupShareClient.TranslationMemories.GetNumberOfTus(tmId, languageParameters);

            Assert.True(tusNumber>0);
        }

        [Theory]
        [InlineData("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530")]
        public async Task GetTusNumberForPostDatedTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var languageParameters = new LanguageParameters("de-de", "ro-ro");

            var tusNumber = await groupShareClient.TranslationMemories.GetNumberOfPostDatedTus(tmId, languageParameters);

            Assert.Equal(tusNumber ,0);
        }

        [Theory]
        [InlineData("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530")]
        public async Task GetTusNumberForPreDatedTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var languageParameters = new LanguageParameters("de-de", "ro-ro");

            var tusNumber = await groupShareClient.TranslationMemories.GetNumberOfPreDatedTus(tmId, languageParameters);

            Assert.Equal(tusNumber, 0);
        }
        [Theory]
        [InlineData("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530")]
        public async Task GetTusNumberForUnalignedTm(string tmId)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            var languageParameters = new LanguageParameters("de-de", "ro-ro");

            var tusNumber = await groupShareClient.TranslationMemories.GetNumberOfUnalignedTus(tmId, languageParameters);

            Assert.Equal(tusNumber, 0);
        }

        [Fact]
        public async Task Filter()
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            //      var languageDetails = new LanguageDetailsRequest("Europäischen", "de-de", "Acord ", "ro-ro");
            var languageDetails = new LanguageDetailsRequest("", "de-de", "Informare", "ro-ro");
            var tmDetails = new TranslationMemoryDetailsRequest(new Guid("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530"),0,50);

            var filter = await groupShareClient.TranslationMemories.FilterAsPlainText(languageDetails, tmDetails,true,true);

            foreach (var segment in filter)
            {
                Assert.True(segment.Target.Contains("Informare"));
            }

        }

        [Theory]
        [InlineData("4b45a229-ea3f-4a2f-bce4-04cf5fdc3530", " \"TestField\" = \"andrea\"",
            "(\"TestField\" = \"andrea\" | \"Second\" = \"Test\")")]
        public async Task CustomFilterExpression(string tmid, string simpleExpression, string customExpression)
        {
            var groupShareClient = await Helper.GetGroupShareClient();
            // simple expression 
            var filedsList = new List<FieldFilter>
            {
                new FieldFilter
                {
                    Name = "TestField",
                    Type = FieldFilter.TypeEnum.SingleString,
                    Values = null
                }
            };

            var filterRequest = new FieldFilterRequest(filedsList, simpleExpression);

            var rawFilterRequest = new RawFilterRequest(new Guid(tmid), "de-de", "ro-ro", 0, 30, filterRequest);

            var responseSimpleExpr = await groupShareClient.TranslationMemories.RawFilter(rawFilterRequest);

            foreach (var item in responseSimpleExpr)
            {
                Assert.Equal(item.Target, "TRADUCERE");
            }

            //custom expression
            var customFiledsList = new List<FieldFilter>
            {
                new FieldFilter
                {
                    Name = "TestField",
                    Type = FieldFilter.TypeEnum.SingleString,
                    Values = null
                },
                new FieldFilter
                {
                    Name = "Second",
                    Type = FieldFilter.TypeEnum.SingleString,
                    Values = null

                }
            };

            var customFilterRequest = new FieldFilterRequest(customFiledsList, customExpression);

            var customRawFilterRequest = new RawFilterRequest(new Guid(tmid), "de-de", "ro-ro", 0, 30,
                customFilterRequest);

            var responseCustomExpr = await groupShareClient.TranslationMemories.RawFilter(customRawFilterRequest);

            Assert.Equal(responseCustomExpr.Count, 2);


        }
    }
}
