﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCodeGenerator.OpenApi.Refit;
using static ApiCodeGenerator.Tests.Helpers;

namespace ApiCodeGenerator.OpenApi.Refit.Tests
{
    public class FormDataTests
    {
        [Test]
        public async Task FormData()
        {
            var json = "{" + OpenApiDocumentDeclaration + @"
  ""paths"": {
    ""/test"": {
      ""post"": {
        ""operationId"": ""GetTest"",
        ""requestBody"": {
            ""content"":{
                ""multipart/form-data"":{
                    ""schema"":{
                        ""type"": ""object"",
                        ""properties"": {
                            ""testProp"":{""type"":""string""}
                        },
                        ""additionalProperties"": false
                    },
                    ""encoding"":{""testProp"": {""style"":""form""}}
                }
            }
        },
        ""responses"": {
          ""200"": {""description"": ""valid input""}
        }
      }
    }
  }}";
            var document = await OpenApiDocument.FromJsonAsync(json);
            RefitCodeGeneratorSettings settings = new()
            {
                GenerateClientInterfaces = true,
                CSharpGeneratorSettings =
                {
                    Namespace = "TestNS",
                },
            };
            var expectedInterfaceCode =
                "    public partial interface IClient\n" +
                "    {\n" +
                "        /// <returns>valid input</returns>\n" +
                "        /// <exception cref=\"Refit.ApiException\">A server side error occurred.</exception>\n" +
                "        [Post(\"/test\")]\n" +
                $"        System.Threading.Tasks.Task GetTest([Body(BodySerializationMethod.UrlEncoded)]GetTestFormData getTestFormData);\n" +
                "\n" +
                "    }\n";
            var expectedClassCode =
               $"    {GENERATED_CODE}\n" +
                "    public partial class GetTestFormData\n" +
                "    {\n" +
                "        [Newtonsoft.Json.JsonProperty(\"testProp\", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]\n" +
                "        public string TestProp { get; set; }\n" +
                "\n" +
                "    }\n";
            RunTest(settings, expectedInterfaceCode, document, expectedClassCode);
        }
    }
}