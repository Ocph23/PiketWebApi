﻿using CommunityToolkit.Mvvm.ComponentModel;
using SharedModel;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Models
{
   public class StudentToLateAndHomeEarlyModel : ObservableObject
    {
        public int Id { get; set; }

        private Student? student;

        public Student? Student
        {
            get { return student; }
            set { SetProperty(ref student , value); }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description , value); }
        }

        private TimeSpan atTime;

        public TimeSpan AtTime
        {
            get { return atTime; }
            set {SetProperty(ref atTime , value); }
        }


        private Teacher createdBy;

        public Teacher CreatedBy
        {
            get { return createdBy; }
            set { SetProperty(ref createdBy , value); }
        }

        public DateTime CreateAt { get; set; } = DateTime.Now;


        private StatusKehadiran statusKehadiran;

        public StatusKehadiran StatusKehadiran
        {
            get { return statusKehadiran; }
            set { SetProperty(ref statusKehadiran , value); }
        }



    }
}
