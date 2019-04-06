namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.Datasets.ExportDtos;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var GamesByGenre = context.Genres
                .Where(genre => genreNames.Contains(genre.Name))
                .Select(genre => new
                {
                    Id = genre.Id,
                    Genre = genre.Name,
                    Games = genre.Games
                    .Where(game => game.Purchases.Any())
                    .Select(game => new
                    {
                        Id = game.Id,
                        Title = game.Name,
                        Developer = game.Developer.Name,
                        Tags = (string.Join(", ", game.GameTags.Select(z => z.Tag.Name))),
                        Players = game.Purchases.Count
                    })
                    .OrderByDescending(g => g.Players)
                    .ThenBy(g => g.Id)
                    .ToList(),
                    TotalPlayers = genre.Games.Sum(g => g.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            return JsonConvert.SerializeObject(GamesByGenre, Newtonsoft.Json.Formatting.Indented);
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
            var type = (PurchaseType)Enum.Parse(typeof(PurchaseType), storeType);

            var users = context.Users
                .Select(x => new ExportUserDto
                {
                    UserName = x.Username,
                    PurchaseExportDto = x.Cards
                        .SelectMany(y => y.Purchases)
                        .Where(t => t.Type == type)
                        .Select(y => new PurchaseExportDto
                        {
                            Card = y.Card.Number,
                            Cvc = y.Card.Cvc,
                            Date = y.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new ExportGameDto
                            {
                                Title = y.Game.Name,
                                Genre = y.Game.Genre.Name,
                                Price = y.Game.Price
                            }
                        })
                    .OrderBy(y => y.Date)
                    .ToArray(),
                    TotalSpent = x.Cards.SelectMany(z => z.Purchases)
                        .Where(z => z.Type == type)
                        .Sum(p => p.Game.Price)
                })
                .Where(p => p.PurchaseExportDto.Any())
                .OrderByDescending(x => x.TotalSpent)
                .ThenBy(x => x.UserName)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), users, namespaces);

            return result.ToString().TrimEnd();
        }
	}
}