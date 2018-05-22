using System.ComponentModel.DataAnnotations;

namespace ShitForum.Models
{
    public class AddBoard
    {
        public AddBoard()
        {
        }

        public AddBoard(string boardName, string boardKey)
        {
            BoardName = boardName;
            BoardKey = boardKey;
        }

        [Required] public string BoardName { get; set; }

        [Required] public string BoardKey { get; set; }
    }
}
