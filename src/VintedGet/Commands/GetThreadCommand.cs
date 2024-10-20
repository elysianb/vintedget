﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetThreadCommand
    {
        public void Execute(string token, string threadId)
        {
            string userId = null; 
            string userCookie = null;
            if (!string.IsNullOrEmpty(token))
            {
                var jwt = new JwtToken(token);
                userId = jwt.UserId;
                userCookie = jwt.TokenCookie;
            }

            VintedProcessor.EnsureHasSession(ref userId, ref userCookie);
            if (VintedProcessor.HasSession(userId, userCookie))
            {
                VintedProcessor.GetThreadImages(userId, userCookie, threadId);
            }
        }
    }
}
