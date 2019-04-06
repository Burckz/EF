namespace BookShop
{
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);
                
            }
        }

        public static int RemoveBooks(BookShopContext db)
        {
            var booksToRemove = db.Books
                .Where(x => x.Copies < 4200)
                .ToList();
            int count = booksToRemove.Count();

            db.RemoveRange(booksToRemove);
            db.SaveChanges();

            return count;
        }

        public static void IncreasePrices(BookShopContext db)
        {
            var bookPrices = db.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in bookPrices)
            {
                book.Price += 5;
            }

            db.SaveChanges();
        }

        public static string GetMostRecentBooks(BookShopContext db)
        {
            StringBuilder sb = new StringBuilder();

            var mostRecentBooks = db.Categories
                .Select(c => new
                {
                    categoryName = c.Name,
                    topThreeMostRecent = c.CategoryBooks.OrderByDescending(cb => cb.Book.ReleaseDate).Select(cb => new
                    {
                        cb.Book.Title,
                        cb.Book.ReleaseDate
                    }).Take(3)
                })
                .OrderBy(c => c.categoryName)
                .ToList();

            foreach (var category in mostRecentBooks)
            {
                sb.AppendLine($"--{category.categoryName}");

                foreach(var book in category.topThreeMostRecent)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext db)
        {
            StringBuilder sb = new StringBuilder();

            var profitByCategory = db.Categories
                .Select(c => new
                {
                    Name = c.Name,
                    totalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.totalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var catergory in profitByCategory)
            {
                sb.AppendLine($"{catergory.Name} ${catergory.totalProfit:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string CountCopiesByAuthor(BookShopContext db)
        {
            StringBuilder sb = new StringBuilder();

            var authors = db.Authors
                .Select(a => new
                {
                    fullName = a.FirstName + " " + a.LastName,
                    bookCopies = a.Books.Sum(book => book.Copies)
                })
                .OrderByDescending(a => a.bookCopies)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.fullName} - {author.bookCopies}");
            }

            return sb.ToString().Trim();
        }

        public static int CountBooks(BookShopContext db, int lengthCheck)
        {
            

            return db.Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToList()
                .Count;
        }

        public static string GetBooksByAuthor(BookShopContext db, string input)
        {
            StringBuilder sb = new StringBuilder();

            var booksWithAuthors = db.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new 
                {
                    b.Title,
                    authorName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            foreach (var book in booksWithAuthors)
            {
                sb.AppendLine($"{book.Title} ({book.authorName})");
            }

            return sb.ToString().Trim();
        }

        public static string GetBookTitlesContaining(BookShopContext db, string input)
        {
            StringBuilder sb = new StringBuilder();

            var titles = db.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToList();


            foreach (var title in titles)
            {
                sb.AppendLine($"{title}");
            }

            return sb.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext db, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = db.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(b =>
                    b.FirstName + " " + b.LastName
                )
                .OrderBy(name => name)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksReleasedBefore(BookShopContext db, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime dateToCheck = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = db.Books
                .Where(b => b.ReleaseDate < dateToCheck)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByCategory(BookShopContext db, string input)
        {
            StringBuilder sb = new StringBuilder();

            List<string> inputCategories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(c => c.ToLower()).ToList();

            var books = db.Books
                .Where(b => b.BookCategories.Any(c => inputCategories.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToList();

            foreach (var title in books)
            {
                sb.AppendLine($"{title}");
            }

            return sb.ToString().Trim();
        }

    public static string GetBooksNotReleasedIn(BookShopContext db, int year)
    {
        StringBuilder sb = new StringBuilder();

        var bookNotReleasedInYear = db.Books
            .Where(b => b.ReleaseDate.Value.Year != year)
            .OrderBy(b => b.BookId)
            .Select(b => new { b.Title })
            .ToList();

        foreach (var book in bookNotReleasedInYear)
        {
            sb.AppendLine($"{book.Title}");
        }

        return sb.ToString().Trim();
    }

    public static string GetBooksByPrice(BookShopContext db)
    {
        StringBuilder sb = new StringBuilder();

        var books = db.Books
            .Where(b => b.Price > 40)
            .Select(b => new
            {
                b.Title,
                b.Price
            })
            .OrderByDescending(x => x.Price)
            .ToList();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} - ${book.Price:f2}");
        }

        return sb.ToString().Trim();
    }

    public static string GetGoldenBooks(BookShopContext bookShopContext)
    {
        var goldenBooks = bookShopContext.Books
            .Where(x => x.Copies < 5000 && x.EditionType.ToString() == "Gold")
            .OrderBy(x => x.BookId)
            .Select(x => x.Title)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var title in goldenBooks)
        {
            sb.AppendLine(title);
        }
        return sb.ToString().TrimEnd();
    }

    public static string GetBooksByAgeRestriction(BookShopContext context, string command)
    {
        string formattedCommand = command.First().ToString().ToUpper() + command.Substring(1).ToLower();
        var restrictedBooks = context.Books
            .Where(x => x.AgeRestriction.ToString() == formattedCommand)
            .OrderBy(x => x.Title)
            .Select(x => x.Title)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var title in restrictedBooks)
        {
            sb.AppendLine(title);
        }
        return sb.ToString().TrimEnd();
    }
}
}
