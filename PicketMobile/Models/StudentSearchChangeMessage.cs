using CommunityToolkit.Mvvm.Messaging.Messages;
using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Models
{
    internal class StudentSearchChangeMessage : ValueChangedMessage<StudentResponse>
    {
        public StudentSearchChangeMessage(StudentResponse value) : base(value)
        {
        }
    }



}
