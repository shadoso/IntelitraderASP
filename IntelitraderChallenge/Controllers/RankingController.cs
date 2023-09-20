using System;
using System.Collections.Generic;
using IntelitraderChallenge.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Web.WebPages.Instrumentation;

namespace IntelitraderChallenge.Controllers
{
    public class RankingController : Controller
    {
        private static readonly int NEW_LETTER_VALUE  = 1;
        private static readonly int MOD = 2;
        private static readonly int REVERSE_INDEX = 1;
        private static readonly int END = 0;
        private const int concatenateForward = 0;
        private const int concatenateBackward = 1;



        public Dictionary<char, int> CountSymbols(string text)
        {
            Dictionary<char, int> rankDictionary = new Dictionary<char, int>();

            foreach (char symbol in text)
            {
                if (!char.IsWhiteSpace(symbol))
                {
                    if (rankDictionary.ContainsKey(symbol))
                    {
                        rankDictionary[symbol]++;
                    }
                    else
                    {
                        rankDictionary[symbol] = NEW_LETTER_VALUE;
                    }
                }
            }
            return rankDictionary;
        }

        /// <summary>
        /// ModSort é um algoritmo de ordenação que ordena os valores por grupos que contêm números inteiros positivos que são pares.
        /// Agrupamento: Grupos = {x | x > 0, x ∈ ℤ, x ≡ 0 (mod 2)}.
        /// </summary>
        /// <remarks>
        /// Por exemplo, os números 2 e 3 são transformados em 4 e, em seguida, adicionados ao Grupo4. Todo número ímpar é concatenado no início porque ele é maior.
        /// Melhor caso é quando os números estão próximos uns dos outros e quando existem várias duplicatas.
        /// Pior caso ocorre quando os números estão a mais de 2 casas de distância e são únicos, o que faz com que o dicionário tenha o mesmo número de elementos.
        /// </remarks>
        public string ModSort(Dictionary<char, int> rankDictionary)
        {
            string text = "";

            Dictionary<int, string> uniqueDictionary = new Dictionary<int, string>();
            List<int> listToString = new List<int>();

            foreach (var similarIndex in rankDictionary)
            {
                // Aqui preparamos todos os nossos números tornando-os únicos e múltiplos de 2.
                int newKey = (similarIndex.Value + MOD) - (similarIndex.Value % MOD);
                var next = similarIndex.Key;

                if (uniqueDictionary.ContainsKey(newKey))
                {
                    var current = uniqueDictionary[newKey];
                    var concatenate = similarIndex.Value % MOD;

                    switch (concatenate)
                    {
                        // Nosso Threshold que ordena todos os valores que estão próximos.
                        case concatenateForward:
                            uniqueDictionary[newKey] = current + next;
                            break;

                        case concatenateBackward:
                            uniqueDictionary[newKey] = next + current;
                            break;
                    }
                }
                else
                {
                    uniqueDictionary[newKey] = next.ToString();

                    // Como os valores são sempre únicos e não existem na lista, o BinarySearch sempre vai retornar o índice que o número deve ser adicionado.
                    int index = listToString.BinarySearch(newKey);
                    index = ~index;
                    listToString.Insert(index, newKey);
                }
            }

            // Acessamos os números de forma reversa para obter o rank em ordem decrescente.
            for (int position = listToString.Count - REVERSE_INDEX; position >= END; position--)
            {
                text += uniqueDictionary[listToString[position]];
            }

            return text;
        }
        // GET: Ranking
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Rank(RankingFile rankingFile)
        {
            string text;

            using (var streamReader = new StreamReader(rankingFile.File.InputStream))
            {
                text = streamReader.ReadToEnd();
            }

            var rankDictionary = CountSymbols(text);
            var rank = ModSort(rankDictionary);

            text = null;

            foreach (var letter in rank)
            {
                text += $"Letra: {letter}       Contagem: {rankDictionary[letter]}\n";
            }

            ViewBag.Rank = text;

                return View();
        }
    }
}