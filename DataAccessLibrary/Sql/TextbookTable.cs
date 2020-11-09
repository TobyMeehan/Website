using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class TextbookTable : MultiMappingTableBase<Textbook>
    {
        public TextbookTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {

        }

        protected override ISqlQuery<Textbook> GetQuery(Dictionary<string, Textbook> dictionary)
        {
            return new SqlQuery<Textbook>()
                .Select()
                .InnerJoin<Chapter>((t, c) => c.TextbookId == t.Id)
                .InnerJoin<Chapter, Exercise>((c, e) => e.ChapterId == c.Id)
                .Map<Chapter, Exercise>((textbook, chapter, exercise) =>
                {
                    if (!dictionary.TryGetValue(textbook.Id, out Textbook entry))
                    {
                        entry = textbook;
                        entry.Chapters = new EntityCollection<Chapter>();

                        dictionary.Add(entry.Id, entry);
                    }

                    if (!entry.Chapters.TryGetItem(chapter.Id, out Chapter chapterEntry))
                    {
                        chapterEntry = chapter;
                        chapterEntry.Exercises = new EntityCollection<Exercise>();
                    }

                    if (!chapterEntry.Exercises.TryGetItem(exercise.Id, out Exercise exerciseEntry))
                    {
                        exerciseEntry = exercise;
                    }

                    return entry;
                });
        }
    }
}
