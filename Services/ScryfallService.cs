using System.Text.Json;
using Projekt_mtg.DTOs;

namespace Projekt_mtg.Services
{
    public class ScryfallService
    {   
        //sends HTTP-requests to API
        private readonly HttpClient _httpClient;

        public ScryfallService(HttpClient httpClient)
        {
             _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Add(
                "Accept",
                "*/*");
                
            //required by Scryfall
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "ProjektMTG/1.0");
        }

        //GET: fetch exact card by card name
        public async Task<CardDto?> GetCardByName(string cardName)
        {
            var url = $"https://api.scryfall.com/cards/named?exact={Uri.EscapeDataString(cardName)}";

            var response = await _httpClient.GetAsync(url);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using JsonDocument doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            //tie response to CardDTO
            //some cards may not have OracleText or ImageUri
            return new CardDto
            {
                Name = root.GetProperty("name").GetString(),
                ManaCost = root.GetProperty("mana_cost").GetString(),
                TypeLine = root.GetProperty("type_line").GetString(),
                OracleText = root.TryGetProperty("oracle_text", out var oracle) ? oracle.GetString() : "",
                Rarity = root.GetProperty("rarity").GetString(),
                ImageUri = root.TryGetProperty("image_uris", out var imageUris) ? imageUris.GetProperty("normal").GetString() : ""
            };
        }

        //GET: search for a card - autocomplete card names
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


        //GET: random cards (used for get 12 random card for home page)
        public async Task<CardDto?> RandomCardHome()
        {
            var url = "https://api.scryfall.com/cards/random";

            var response = await _httpClient.GetAsync(url);

            if(!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            return new CardDto
                {
                    Name = root.GetProperty("name").GetString(),
                    ManaCost = root.TryGetProperty("mana_cost", out var mana) ? mana.GetString() : "",
                    TypeLine = root.GetProperty("type_line").GetString(),
                    OracleText = root.TryGetProperty("oracle_text", out var oracle) ? oracle.GetString() : "",
                    Rarity = root.GetProperty("rarity").GetString(),
                    ImageUri = root.TryGetProperty("image_uris", out var imageUris) ? imageUris.GetProperty("normal").GetString() : ""
                };
        }


        //GET: when user searches on home page, search from Scryfall's database
        public async Task<List<CardDto>> SearchScryfall(string query)
        {
            var url = $"https://api.scryfall.com/cards/search?q={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<CardDto>();
            }

            var json = await response.Content.ReadAsStringAsync(); 
            using JsonDocument doc = JsonDocument.Parse(json);

            var cards = new List<CardDto>();

            //loop through cards
            foreach(var item in doc.RootElement.GetProperty("data").EnumerateArray())
            {
                string imageUri = "";

                if(item.TryGetProperty("image_uris", out var imageUris))
                {
                    imageUri = imageUris.GetProperty("normal").GetString();
                }

                cards.Add(new CardDto
                {
                    Name = item.GetProperty("name").GetString(),
                    ManaCost = item.TryGetProperty("mana_cost", out var mana) ? mana.GetString() : "",
                    TypeLine = item.GetProperty("type_line").GetString(),
                    OracleText = item.TryGetProperty("oracle_text", out var oracle) ? oracle.GetString() : "",
                    Rarity = item.GetProperty("rarity").GetString(),
                    ImageUri = imageUri
                });

               
            }

             return cards;

        }
    }

}