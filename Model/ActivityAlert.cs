﻿/*
ZoneFox Business Layer event processor based on GNU NEsper
Copyright (C) 2018 ZoneFox

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;

namespace ZF.BL.Nesper.Model
{
    public class ActivityAlert
    {
        public ActivityAlert()
        {
        }

        //retain for serialiser

        public ActivityAlert(string ruleId, DateTime occurredOn, ActivityEvent[] events)
        {
            RuleId = ruleId;
            OccurredOn = occurredOn;
            Events = events;
        }

        public string RuleId { get; set; }
        public DateTime OccurredOn { get; set; }
        public ActivityEvent[] Events { get; set; }
    }
}