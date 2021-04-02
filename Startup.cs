using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace endpoints
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {  
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            string text = "I need food, food is my life! Life bad without food, i love food.";   
            Dictionary<string, int> dict = WordFrequency(text);

            int cho = CountUniqWords(dict);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/headers", async context =>
                {
                    string data = "";
                    foreach (var header in context.Request.Headers)
                    {
                        data += $"{header.Key}: {header.Value}\n";
                    }
                    await context.Response.WriteAsync(data);
                });

                endpoints.MapGet("/pular", async context =>
                {
                    int index = Convert.ToInt32(context.Request.Query["number"]);
                    string[] forms = context.Request.Query["forms"].ToString().Split(',');
                    await context.Response.WriteAsync($"{index} {Plural(index, forms[0], forms[1], forms[2])} \n ");
                });

                endpoints.MapPost("/frequency", async context =>
                {
                    StreamReader reader = new StreamReader(context.Request.Body);
                    string data = reader.ReadToEnd();
                    
                    Dictionary<string, int> frequencyDict = WordFrequency(data);
                    string uniqWords = (CountUniqWords(frequencyDict)).ToString();
                    string frequenWord = FindMostFrequenWord(dict);

                    context.Response.Headers.Add("Content-Type","application/json");
                    context.Response.Headers.Add("Unique-Words", uniqWords);
                    context.Response.Headers.Add("Most-Frequen-Word", frequenWord);

                    await context.Response.WriteAsJsonAsync(frequencyDict);
                    //await context.Response.WriteAsync(""); 
                });
            });
        }


        static Dictionary<string, int> WordFrequency(string text)
        {   
            string tempText = "";
            string[] massText;
            
            int counter = 0;
            
            Dictionary<string, int> dict = new Dictionary<string, int>();
            
            text = text.ToLower();
            massText = text.Split(" ");
            
            
            foreach (string element in massText)
            {
                tempText = "";
                
                for (int i = 0; i < element.Length; i++)
                    if (Char.IsLetter(element, i))
                        tempText += element[i];

                massText[counter] = tempText;
                counter++;
            }
            
            foreach (string element in massText)
            {   
                if (dict.ContainsKey(element))
                    dict[element]++;
                else
                    dict.Add(element, 1);
            }
            return dict;
        }

        static int CountUniqWords(Dictionary<string, int> dict)
        {
            int counter = 0;
            var cho = dict.Values; 
            foreach(var element in cho)
            {
                if (element == 1)
                    counter++;
            }
            return counter;
        }

        static string FindMostFrequenWord(Dictionary<string, int> dict)
        {
            int check = 0;
            string word = ""; 
            foreach(var element in dict)
            {
                if (element.Value > check)
                    check = element.Value;
                    word = element.Key;
            }
            return word;
        }


        static string Plural(int num, string one, string couple, string many)
        {
            // тут должна быть моя функция пюляризации но она осталась в офисе
            return "";
        }
    }
}
