﻿using System;
using System.Collections.Generic;
using System.Text;
using UnitechMobileApp.ScheduleHelper;

namespace UnitechMobileApp.Model
{
    public class TeacherBehavior : IUser
    {
        public Dictionary<int, List<ScheduleLesson>> GetSchedule()
        {
            throw new NotImplementedException();
        }
    }
}
