namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.Datasets.ImportDtos;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gamesDto = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            StringBuilder result = new StringBuilder();

            foreach (var gameDto in gamesDto)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                Developer developer = context.Developers.FirstOrDefault(d => d.Name == gameDto.Developer);

                if (developer == null)
                {
                    developer = new Developer { Name = gameDto.Developer };
                    context.Developers.Add(developer);
                }

                Genre genre = context.Genres.FirstOrDefault(g => g.Name == gameDto.Genre);

                if (genre == null)
                {
                    genre = new Genre { Name = gameDto.Genre };
                    context.Genres.Add(genre);
                }

                Game game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = developer,
                    Genre = genre,
                    GameTags = new List<GameTag>()
                };

                foreach (var ta in gameDto.Tags)
                {
                    Tag tag = context.Tags.FirstOrDefault(t => t.Name == ta);

                    if (tag == null)
                    {
                        tag = new Tag { Name = ta };
                        context.Tags.Add(tag);
                    }

                    game.GameTags.Add(new GameTag { Tag = tag });
                }

                context.Games.Add(game);
                result.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
                context.SaveChanges();
            }

            return result.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var users = JsonConvert.DeserializeObject<User[]>(jsonString);

            StringBuilder result = new StringBuilder();

            foreach (var user in users)
            {
                if(!IsValid(user) || user.Cards.Count == 0)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                User newUser = new User
                {
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Age = user.Age,
                    Cards = new List<Card>()
                };

                foreach (var card in user.Cards)
                {
                    if (!IsValid(card))
                    {
                        result.AppendLine("Invalid Data");
                        continue;
                    }

                    Card newCard = new Card
                    {
                        Number = card.Number,
                        Cvc = card.Cvc,
                        Type = card.Type,
                        User = newUser,
                        Purchases = new List<Purchase>()
                    };

                    newUser.Cards.Add(newCard);
                }

                context.Users.Add(newUser);
                result.AppendLine($"Imported {newUser.Username} with {newUser.Cards.Count} cards");
            }

            context.SaveChanges();
            return result.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            var serializer = new XmlSerializer(typeof(PurchaseDto[]), new XmlRootAttribute("Purchases"));

            var purchases = (PurchaseDto[])serializer.Deserialize(new StringReader(xmlString));

            foreach (var purchase in purchases)
            {
                if (!IsValid(purchase) || !Enum.IsDefined(typeof(PurchaseType), purchase.Type))
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }


                var game = context.Games.FirstOrDefault(g => g.Name == purchase.Game);
                var card = context.Cards.FirstOrDefault(c => c.Number == purchase.Card);


                

                if(game == null || card == null)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                var newPurchase = new Purchase
                {
                    Type = (PurchaseType)Enum.Parse(typeof(PurchaseType), purchase.Type),
                    ProductKey = purchase.ProductKey,
                    Date = DateTime.ParseExact(purchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Game = game,
                    Card = card
                };

                result.AppendLine($"Imported {newPurchase.Game.Name} for {newPurchase.Card.User.Username}");
                context.Purchases.Add(newPurchase);

            }

            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return isValid;
        }
    }
}