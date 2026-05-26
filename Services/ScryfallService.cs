using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Mvc.Routing;
using Projekt_mtg.DTOs;

namespace Projekt_mtg.Services
{
    public class ScryfallService
    {
        private readonly HttpClient _httpClient;

        public ScryfallService(HttpClient httpClient)
        {
             _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "ProjektMTG/1.0");

            _httpClient.DefaultRequestHeaders.Add(
                "Accept",
                "*/*");
        }

        //hämtar kort efter kortnamn
        public async Task<CardDto?> GetCardByName(string cardName)
        {
            var url = $"https://api.scryfall.com/cards/named?exact={Uri.EscapeDataString(cardName)}";

            var response = await _httpClient.GetAsync(url);

            //felhantering
            var errorContent = await response.Content.ReadAsStringAsync();

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"STATUS: {response.StatusCode}");
            Console.WriteLine(json);

            if (!response.IsSuccessStatusCode)
            {
                return null;
}

            using JsonDocument doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            return new CardDto
            {
                Name = root.GetProperty("name").GetString(),
                ManaCost = root.GetProperty("mana_cost").GetString(),
                TypeLine = root.GetProperty("type_line").GetString(),
                OracleText = root.TryGetProperty("oracle_text", out var oracle)
                    ? oracle.GetString()
                    : "",
                Rarity = root.GetProperty("rarity").GetString(),
                ImageUri = root
                    .GetProperty("image_uris")
                    .GetProperty("normal")
                    .GetString()
            };
        }

        //söker efter kort
        public async Task<List<string>> SearchCards(string query)
        {
            var url = $"https://api.scryfall.com/cards/autocomplete?q={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);

            if(!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("data")
                .EnumerateArray()
                .Select(x => x.GetString()!)
                .ToList();
        }
    }

}