using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Interfaces.Chat
{
    public interface IChatUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }
    }
}
