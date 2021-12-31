﻿using ProjectWorldCup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.WorldCup
{
    public class LeagueTest
    {
        [Fact]
        public void PremierLeague_2020_2021()
        {
            var league = new League
            {
                Name = "PremierLeague 2020-2021",
                Teams = new(),
                Matches = new(),
            };

            #region Teams
            league.Teams.Add(new Team { Id = "MCI", Name = "Manchester City" });
            league.Teams.Add(new Team { Id = "MUN", Name = "Manchester United" });
            league.Teams.Add(new Team { Id = "LIV", Name = "Liverpool" });
            league.Teams.Add(new Team { Id = "CHE", Name = "Chelsea" });
            league.Teams.Add(new Team { Id = "LEI", Name = "Leicester City" });
            league.Teams.Add(new Team { Id = "WHU", Name = "West Ham United" });
            league.Teams.Add(new Team { Id = "TOT", Name = "Tottenham Hotspur" });
            league.Teams.Add(new Team { Id = "ARS", Name = "Arsenal" });
            league.Teams.Add(new Team { Id = "LEE", Name = "Leeds United" });
            league.Teams.Add(new Team { Id = "EVE", Name = "Everton" });
            league.Teams.Add(new Team { Id = "AVL", Name = "Aston Villa" });
            league.Teams.Add(new Team { Id = "NEW", Name = "Newcastle United" });
            league.Teams.Add(new Team { Id = "WOL", Name = "Wolverhampton Wanderers" });
            league.Teams.Add(new Team { Id = "CRY", Name = "Crystal Palace" });
            league.Teams.Add(new Team { Id = "SOU", Name = "Southampton" });
            league.Teams.Add(new Team { Id = "BHA", Name = "Brighton and Hove Albion" });
            league.Teams.Add(new Team { Id = "BUR", Name = "Burnley" });
            league.Teams.Add(new Team { Id = "FUL", Name = "Fulham" });
            league.Teams.Add(new Team { Id = "WBA", Name = "West Bromwich Albion" });
            league.Teams.Add(new Team { Id = "SHU", Name = "Sheffield United" });
            #endregion

            #region Matches
            league.AddMatch("ARS", "BHA", 2, 0);
            league.AddMatch("AVL", "CHE", 2, 1);
            league.AddMatch("FUL", "NEW", 0, 2);
            league.AddMatch("LEE", "WBA", 3, 1);
            league.AddMatch("LEI", "TOT", 2, 4);
            league.AddMatch("LIV", "CRY", 2, 0);
            league.AddMatch("MCI", "EVE", 5, 0);
            league.AddMatch("SHU", "BUR", 1, 0);
            league.AddMatch("WHU", "SOU", 3, 0);
            league.AddMatch("WOL", "MUN", 1, 2);
            league.AddMatch("BUR", "LIV", 0, 3);
            league.AddMatch("WBA", "WHU", 1, 3);
            league.AddMatch("CRY", "ARS", 1, 3);
            league.AddMatch("EVE", "WOL", 1, 0);
            league.AddMatch("NEW", "SHU", 1, 0);
            league.AddMatch("TOT", "AVL", 1, 2);
            league.AddMatch("CHE", "LEI", 2, 1);
            league.AddMatch("BHA", "MCI", 3, 2);
            league.AddMatch("MUN", "FUL", 1, 1);
            league.AddMatch("SOU", "LEE", 0, 2);
            league.AddMatch("EVE", "SHU", 0, 1);
            league.AddMatch("WBA", "LIV", 1, 2);
            league.AddMatch("TOT", "WOL", 2, 0);
            league.AddMatch("CRY", "AVL", 3, 2);
            league.AddMatch("BHA", "WHU", 1, 1);
            league.AddMatch("SOU", "FUL", 3, 1);
            league.AddMatch("BUR", "LEE", 0, 4);
            league.AddMatch("NEW", "MCI", 3, 4);
            league.AddMatch("MUN", "LIV", 2, 4);
            league.AddMatch("AVL", "EVE", 0, 0);
            league.AddMatch("CHE", "ARS", 0, 1);
            league.AddMatch("SOU", "CRY", 3, 1);
            league.AddMatch("MUN", "LEI", 1, 2);
            league.AddMatch("FUL", "BUR", 0, 2);
            league.AddMatch("ARS", "WBA", 3, 1);
            league.AddMatch("WHU", "EVE", 0, 1);
            league.AddMatch("AVL", "MUN", 1, 3);
            league.AddMatch("WOL", "BHA", 2, 1);
            league.AddMatch("LIV", "SOU", 2, 0);
            league.AddMatch("MCI", "CHE", 1, 2);
            league.AddMatch("SHU", "CRY", 0, 2);
            league.AddMatch("LEE", "TOT", 3, 1);
            league.AddMatch("LEI", "NEW", 2, 4);
            league.AddMatch("BUR", "WHU", 1, 2);
            league.AddMatch("WBA", "WOL", 1, 1);
            league.AddMatch("TOT", "SHU", 4, 0);
            league.AddMatch("NEW", "ARS", 0, 2);
            league.AddMatch("EVE", "AVL", 1, 2);
            league.AddMatch("CHE", "FUL", 2, 0);
            league.AddMatch("BHA", "LEE", 2, 0);
            league.AddMatch("CRY", "MCI", 0, 2);
            league.AddMatch("SOU", "LEI", 1, 1);
            league.AddMatch("LEI", "CRY", 2, 1);
            league.AddMatch("AVL", "WBA", 2, 2);
            league.AddMatch("LEE", "MUN", 0, 0);
            league.AddMatch("WOL", "BUR", 0, 4);
            league.AddMatch("SHU", "BHA", 1, 0);
            league.AddMatch("WHU", "CHE", 0, 1);
            league.AddMatch("LIV", "NEW", 1, 1);
            league.AddMatch("ARS", "EVE", 0, 1);
            league.AddMatch("LEI", "WBA", 3, 0);
            league.AddMatch("AVL", "MCI", 1, 2);
            league.AddMatch("TOT", "SOU", 2, 1);
            league.AddMatch("CHE", "BHA", 0, 0);
            league.AddMatch("LEE", "LIV", 1, 1);
            league.AddMatch("MUN", "BUR", 3, 1);
            league.AddMatch("ARS", "FUL", 1, 1);
            league.AddMatch("WOL", "SHU", 1, 0);
            league.AddMatch("NEW", "WHU", 3, 2);
            league.AddMatch("EVE", "TOT", 2, 2);
            league.AddMatch("BHA", "EVE", 0, 0);
            league.AddMatch("WBA", "SOU", 3, 0);
            league.AddMatch("SHU", "ARS", 0, 3);
            league.AddMatch("TOT", "MUN", 1, 3);
            league.AddMatch("WHU", "LEI", 3, 2);
            league.AddMatch("BUR", "NEW", 1, 2);
            league.AddMatch("CRY", "CHE", 1, 4);
            league.AddMatch("LIV", "AVL", 2, 1);
            league.AddMatch("MCI", "LEE", 1, 2);
            league.AddMatch("FUL", "WOL", 0, 1);
            league.AddMatch("WOL", "WHU", 2, 3);
            league.AddMatch("EVE", "CRY", 1, 1);
            league.AddMatch("MUN", "BHA", 2, 1);
            league.AddMatch("AVL", "FUL", 3, 1);
            league.AddMatch("NEW", "TOT", 2, 2);
            league.AddMatch("SOU", "BUR", 3, 2);
            league.AddMatch("ARS", "LIV", 0, 3);
            league.AddMatch("LEI", "MCI", 0, 2);
            league.AddMatch("LEE", "SHU", 2, 1);
            league.AddMatch("CHE", "WBA", 2, 5);
            league.AddMatch("AVL", "TOT", 0, 2);
            league.AddMatch("WHU", "ARS", 3, 3);
            league.AddMatch("BHA", "NEW", 3, 0);
            league.AddMatch("FUL", "LEE", 1, 2);
            league.AddMatch("WOL", "LIV", 0, 1);
            league.AddMatch("MUN", "WHU", 1, 0);
            league.AddMatch("ARS", "TOT", 2, 1);
            league.AddMatch("LEI", "SHU", 5, 0);
            league.AddMatch("SOU", "BHA", 1, 2);
            league.AddMatch("FUL", "MCI", 0, 3);
            league.AddMatch("EVE", "BUR", 1, 2);
            league.AddMatch("CRY", "WBA", 1, 0);
            league.AddMatch("LEE", "CHE", 0, 0);
            league.AddMatch("NEW", "AVL", 1, 1);
            league.AddMatch("MCI", "SOU", 5, 2);
            league.AddMatch("WHU", "LEE", 2, 0);
            league.AddMatch("CHE", "EVE", 2, 0);
            league.AddMatch("TOT", "CRY", 4, 1);
            league.AddMatch("MCI", "MUN", 0, 2);
            league.AddMatch("LIV", "FUL", 0, 1);
            league.AddMatch("WBA", "NEW", 0, 0);
            league.AddMatch("BHA", "LEI", 1, 2);
            league.AddMatch("AVL", "WOL", 0, 0);
            league.AddMatch("SHU", "SOU", 0, 2);
            league.AddMatch("BUR", "ARS", 1, 1);
            league.AddMatch("LIV", "CHE", 0, 1);
            league.AddMatch("FUL", "TOT", 0, 1);
            league.AddMatch("WBA", "EVE", 0, 1);
            league.AddMatch("CRY", "MUN", 0, 0);
            league.AddMatch("BUR", "LEI", 1, 1);
            league.AddMatch("SHU", "AVL", 1, 0);
            league.AddMatch("MCI", "WOL", 4, 1);
            league.AddMatch("EVE", "SOU", 1, 0);
            league.AddMatch("SHU", "LIV", 0, 2);
            league.AddMatch("CHE", "MUN", 0, 0);
            league.AddMatch("TOT", "BUR", 4, 0);
            league.AddMatch("CRY", "FUL", 0, 0);
            league.AddMatch("LEI", "ARS", 1, 3);
            league.AddMatch("NEW", "WOL", 1, 1);
            league.AddMatch("LEE", "AVL", 0, 1);
            league.AddMatch("WBA", "BHA", 1, 0);
            league.AddMatch("MCI", "WHU", 2, 1);
            league.AddMatch("LEE", "SOU", 3, 0);
            league.AddMatch("BHA", "CRY", 1, 2);
            league.AddMatch("MUN", "NEW", 3, 1);
            league.AddMatch("ARS", "MCI", 0, 1);
            league.AddMatch("AVL", "LEI", 1, 2);
            league.AddMatch("WHU", "TOT", 2, 1);
            league.AddMatch("FUL", "SHU", 1, 0);
            league.AddMatch("LIV", "EVE", 0, 2);
            league.AddMatch("BUR", "WBA", 0, 0);
            league.AddMatch("SOU", "CHE", 1, 1);
            league.AddMatch("WOL", "LEE", 1, 0);
            league.AddMatch("EVE", "MCI", 1, 3);
            league.AddMatch("BUR", "FUL", 1, 1);
            league.AddMatch("CHE", "NEW", 2, 0);
            league.AddMatch("WHU", "SHU", 3, 0);
            league.AddMatch("EVE", "FUL", 0, 2);
            league.AddMatch("ARS", "LEE", 4, 2);
            league.AddMatch("WBA", "MUN", 1, 1);
            league.AddMatch("SOU", "WOL", 1, 2);
            league.AddMatch("BHA", "AVL", 0, 0);
            league.AddMatch("MCI", "TOT", 3, 0);
            league.AddMatch("CRY", "BUR", 0, 3);
            league.AddMatch("LEI", "LIV", 3, 1);
            league.AddMatch("LEE", "CRY", 2, 0);
            league.AddMatch("SHU", "CHE", 1, 2);
            league.AddMatch("LIV", "MCI", 1, 4);
            league.AddMatch("WOL", "LEI", 0, 0);
            league.AddMatch("TOT", "WBA", 2, 0);
            league.AddMatch("MUN", "EVE", 3, 3);
            league.AddMatch("FUL", "WHU", 0, 0);
            league.AddMatch("BUR", "BHA", 1, 1);
            league.AddMatch("NEW", "SOU", 3, 2);
            league.AddMatch("AVL", "ARS", 1, 0);
            league.AddMatch("TOT", "CHE", 0, 1);
            league.AddMatch("AVL", "WHU", 1, 3);
            league.AddMatch("LIV", "BHA", 0, 1);
            league.AddMatch("LEE", "EVE", 1, 2);
            league.AddMatch("BUR", "MCI", 0, 2);
            league.AddMatch("FUL", "LEI", 0, 2);
            league.AddMatch("MUN", "SOU", 9, 0);
            league.AddMatch("NEW", "CRY", 1, 2);
            league.AddMatch("SHU", "WBA", 2, 1);
            league.AddMatch("WOL", "ARS", 2, 1);
            league.AddMatch("BHA", "TOT", 1, 0);
            league.AddMatch("WHU", "LIV", 1, 3);
            league.AddMatch("LEI", "LEE", 1, 3);
            league.AddMatch("CHE", "BUR", 2, 0);
            league.AddMatch("SOU", "AVL", 0, 1);
            league.AddMatch("ARS", "MUN", 0, 0);
            league.AddMatch("CRY", "WOL", 1, 0);
            league.AddMatch("MCI", "SHU", 1, 0);
            league.AddMatch("WBA", "FUL", 2, 2);
            league.AddMatch("EVE", "NEW", 0, 2);
            league.AddMatch("TOT", "LIV", 1, 3);
            league.AddMatch("EVE", "LEI", 1, 1);
            league.AddMatch("MUN", "SHU", 1, 2);
            league.AddMatch("BHA", "FUL", 0, 0);
            league.AddMatch("BUR", "AVL", 3, 2);
            league.AddMatch("CHE", "WOL", 0, 0);
            league.AddMatch("SOU", "ARS", 1, 3);
            league.AddMatch("WBA", "MCI", 0, 5);
            league.AddMatch("CRY", "WHU", 2, 3);
            league.AddMatch("NEW", "LEE", 1, 2);
            league.AddMatch("AVL", "NEW", 2, 0);
            league.AddMatch("LIV", "BUR", 0, 1);
            league.AddMatch("FUL", "MUN", 1, 2);
            league.AddMatch("MCI", "AVL", 2, 0);
            league.AddMatch("LEI", "CHE", 2, 0);
            league.AddMatch("WHU", "WBA", 2, 1);
            league.AddMatch("ARS", "NEW", 3, 0);
            league.AddMatch("MCI", "CRY", 4, 0);
            league.AddMatch("LIV", "MUN", 0, 0);
            league.AddMatch("SHU", "TOT", 1, 3);
            league.AddMatch("LEI", "SOU", 2, 0);
            league.AddMatch("FUL", "CHE", 0, 1);
            league.AddMatch("LEE", "BHA", 0, 1);
            league.AddMatch("WHU", "BUR", 1, 0);
            league.AddMatch("WOL", "WBA", 2, 3);
            league.AddMatch("ARS", "CRY", 0, 0);
            league.AddMatch("TOT", "FUL", 1, 1);
            league.AddMatch("MCI", "BHA", 1, 0);
            league.AddMatch("BUR", "MUN", 0, 1);
            league.AddMatch("WOL", "EVE", 1, 2);
            league.AddMatch("SHU", "NEW", 1, 0);
            league.AddMatch("SOU", "LIV", 1, 0);
            league.AddMatch("CHE", "MCI", 1, 3);
            league.AddMatch("NEW", "LEI", 1, 2);
            league.AddMatch("WBA", "ARS", 0, 4);
            league.AddMatch("BHA", "WOL", 3, 3);
            league.AddMatch("CRY", "SHU", 2, 0);
            league.AddMatch("TOT", "LEE", 3, 0);
            league.AddMatch("MUN", "AVL", 2, 1);
            league.AddMatch("EVE", "WHU", 0, 1);
            league.AddMatch("NEW", "LIV", 0, 0);
            league.AddMatch("MUN", "WOL", 1, 0);
            league.AddMatch("BHA", "ARS", 0, 1);
            league.AddMatch("BUR", "SHU", 1, 0);
            league.AddMatch("SOU", "WHU", 0, 0);
            league.AddMatch("WBA", "LEE", 0, 5);
            league.AddMatch("CHE", "AVL", 1, 1);
            league.AddMatch("CRY", "LEI", 1, 1);
            league.AddMatch("WOL", "TOT", 1, 1);
            league.AddMatch("LIV", "WBA", 1, 1);
            league.AddMatch("WHU", "BHA", 2, 2);
            league.AddMatch("LEE", "BUR", 1, 0);
            league.AddMatch("MCI", "NEW", 2, 0);
            league.AddMatch("SHU", "EVE", 0, 1);
            league.AddMatch("ARS", "CHE", 3, 1);
            league.AddMatch("AVL", "CRY", 3, 0);
            league.AddMatch("FUL", "SOU", 0, 0);
            league.AddMatch("LEI", "MUN", 2, 2);
            league.AddMatch("CHE", "WHU", 3, 0);
            league.AddMatch("BUR", "WOL", 2, 1);
            league.AddMatch("WBA", "AVL", 0, 3);
            league.AddMatch("MUN", "LEE", 6, 2);
            league.AddMatch("TOT", "LEI", 0, 2);
            league.AddMatch("BHA", "SHU", 1, 1);
            league.AddMatch("NEW", "FUL", 1, 1);
            league.AddMatch("EVE", "ARS", 2, 1);
            league.AddMatch("SOU", "MCI", 0, 1);
            league.AddMatch("CRY", "LIV", 0, 7);
            league.AddMatch("SHU", "MUN", 2, 3);
            league.AddMatch("AVL", "BUR", 0, 0);
            league.AddMatch("FUL", "BHA", 0, 0);
            league.AddMatch("LIV", "TOT", 2, 1);
            league.AddMatch("WHU", "CRY", 1, 1);
            league.AddMatch("ARS", "SOU", 1, 1);
            league.AddMatch("LEE", "NEW", 5, 2);
            league.AddMatch("LEI", "EVE", 0, 2);
            league.AddMatch("MCI", "WBA", 1, 1);
            league.AddMatch("WOL", "CHE", 2, 1);
            league.AddMatch("ARS", "BUR", 0, 1);
            league.AddMatch("LEI", "BHA", 3, 0);
            league.AddMatch("FUL", "LIV", 1, 1);
            league.AddMatch("CRY", "TOT", 1, 1);
            league.AddMatch("SOU", "SHU", 3, 0);
            league.AddMatch("EVE", "CHE", 1, 0);
            league.AddMatch("MUN", "MCI", 0, 0);
            league.AddMatch("NEW", "WBA", 2, 1);
            league.AddMatch("WOL", "AVL", 0, 1);
            league.AddMatch("LEE", "WHU", 1, 2);
            league.AddMatch("BHA", "SOU", 1, 2);
            league.AddMatch("LIV", "WOL", 4, 0);
            league.AddMatch("TOT", "ARS", 2, 0);
            league.AddMatch("SHU", "LEI", 1, 2);
            league.AddMatch("WBA", "CRY", 1, 5);
            league.AddMatch("CHE", "LEE", 3, 1);
            league.AddMatch("WHU", "MUN", 1, 3);
            league.AddMatch("MCI", "FUL", 2, 0);
            league.AddMatch("BUR", "EVE", 1, 1);
            league.AddMatch("WHU", "AVL", 2, 1);
            league.AddMatch("LEI", "FUL", 1, 2);
            league.AddMatch("ARS", "WOL", 1, 2);
            league.AddMatch("CHE", "TOT", 0, 0);
            league.AddMatch("SOU", "MUN", 2, 3);
            league.AddMatch("WBA", "SHU", 1, 0);
            league.AddMatch("EVE", "LEE", 0, 1);
            league.AddMatch("MCI", "BUR", 5, 0);
            league.AddMatch("BHA", "LIV", 1, 1);
            league.AddMatch("CRY", "NEW", 0, 2);
            league.AddMatch("WOL", "SOU", 1, 1);
            league.AddMatch("BUR", "CRY", 1, 0);
            league.AddMatch("LIV", "LEI", 3, 0);
            league.AddMatch("LEE", "ARS", 0, 0);
            league.AddMatch("SHU", "WHU", 0, 1);
            league.AddMatch("FUL", "EVE", 2, 3);
            league.AddMatch("MUN", "WBA", 1, 0);
            league.AddMatch("TOT", "MCI", 2, 0);
            league.AddMatch("AVL", "BHA", 1, 2);
            league.AddMatch("NEW", "CHE", 0, 2);
            league.AddMatch("ARS", "AVL", 0, 3);
            league.AddMatch("MCI", "LIV", 1, 1);
            league.AddMatch("LEI", "WOL", 1, 0);
            league.AddMatch("WBA", "TOT", 0, 1);
            league.AddMatch("WHU", "FUL", 1, 0);
            league.AddMatch("CHE", "SHU", 4, 1);
            league.AddMatch("CRY", "LEE", 4, 1);
            league.AddMatch("EVE", "MUN", 1, 3);
            league.AddMatch("SOU", "NEW", 2, 0);
            league.AddMatch("BHA", "BUR", 0, 0);
            league.AddMatch("LEE", "LEI", 1, 4);
            league.AddMatch("FUL", "WBA", 2, 0);
            league.AddMatch("TOT", "BHA", 2, 1);
            league.AddMatch("MUN", "ARS", 0, 1);
            league.AddMatch("NEW", "EVE", 2, 1);
            league.AddMatch("AVL", "SOU", 3, 4);
            league.AddMatch("LIV", "WHU", 2, 1);
            league.AddMatch("BUR", "CHE", 0, 3);
            league.AddMatch("SHU", "MCI", 0, 1);
            league.AddMatch("WOL", "CRY", 2, 0);
            league.AddMatch("BUR", "TOT", 0, 1);
            league.AddMatch("BHA", "WBA", 1, 1);
            league.AddMatch("ARS", "LEI", 0, 1);
            league.AddMatch("WOL", "NEW", 1, 1);
            league.AddMatch("SOU", "EVE", 2, 0);
            league.AddMatch("LIV", "SHU", 2, 1);
            league.AddMatch("MUN", "CHE", 0, 0);
            league.AddMatch("FUL", "CRY", 1, 2);
            league.AddMatch("WHU", "MCI", 1, 1);
            league.AddMatch("AVL", "LEE", 0, 3);
            league.AddMatch("LEE", "WOL", 0, 1);
            league.AddMatch("WBA", "BUR", 0, 0);
            league.AddMatch("LEI", "AVL", 0, 1);
            league.AddMatch("TOT", "WHU", 3, 3);
            league.AddMatch("CRY", "BHA", 1, 1);
            league.AddMatch("SHU", "FUL", 1, 1);
            league.AddMatch("NEW", "MUN", 1, 4);
            league.AddMatch("MCI", "ARS", 1, 0);
            league.AddMatch("CHE", "SOU", 3, 3);
            league.AddMatch("EVE", "LIV", 2, 2);
            league.AddMatch("AVL", "LIV", 7, 2);
            league.AddMatch("MUN", "TOT", 1, 6);
            league.AddMatch("ARS", "SHU", 2, 1);
            league.AddMatch("WOL", "FUL", 1, 0);
            league.AddMatch("LEI", "WHU", 0, 3);
            league.AddMatch("SOU", "WBA", 2, 0);
            league.AddMatch("NEW", "BUR", 3, 1);
            league.AddMatch("LEE", "MCI", 1, 1);
            league.AddMatch("EVE", "BHA", 4, 2);
            league.AddMatch("CHE", "CRY", 4, 0);
            league.AddMatch("LIV", "ARS", 3, 1);
            league.AddMatch("FUL", "AVL", 0, 3);
            league.AddMatch("WHU", "WOL", 4, 0);
            league.AddMatch("MCI", "LEI", 2, 5);
            league.AddMatch("TOT", "NEW", 1, 1);
            league.AddMatch("SHU", "LEE", 0, 1);
            league.AddMatch("BUR", "SOU", 0, 1);
            league.AddMatch("WBA", "CHE", 3, 3);
            league.AddMatch("CRY", "EVE", 1, 2);
            league.AddMatch("BHA", "MUN", 2, 3);
            league.AddMatch("WOL", "MCI", 1, 3);
            league.AddMatch("AVL", "SHU", 1, 0);
            league.AddMatch("LEI", "BUR", 4, 2);
            league.AddMatch("CHE", "LIV", 0, 2);
            league.AddMatch("NEW", "BHA", 0, 3);
            league.AddMatch("SOU", "TOT", 2, 5);
            league.AddMatch("ARS", "WHU", 2, 1);
            league.AddMatch("MUN", "CRY", 1, 3);
            league.AddMatch("LEE", "FUL", 4, 3);
            league.AddMatch("EVE", "WBA", 5, 2);
            league.AddMatch("BHA", "CHE", 1, 3);
            league.AddMatch("SHU", "WOL", 0, 2);
            league.AddMatch("TOT", "EVE", 0, 1);
            league.AddMatch("WBA", "LEI", 0, 3);
            league.AddMatch("WHU", "NEW", 0, 2);
            league.AddMatch("LIV", "LEE", 4, 3);
            league.AddMatch("CRY", "SOU", 1, 0);
            league.AddMatch("FUL", "ARS", 0, 3);
            #endregion

            var stands = league.Stands;

            Assert.Equal(league.Teams.Count, stands.Count);

            #region Rank 순위
            var rank = 0;
            Assert.Equal("MCI", stands[rank++].Team.Id);
            Assert.Equal("MUN", stands[rank++].Team.Id);
            Assert.Equal("LIV", stands[rank++].Team.Id);
            Assert.Equal("CHE", stands[rank++].Team.Id);
            Assert.Equal("LEI", stands[rank++].Team.Id);
            Assert.Equal("WHU", stands[rank++].Team.Id);
            Assert.Equal("TOT", stands[rank++].Team.Id);
            Assert.Equal("ARS", stands[rank++].Team.Id);
            Assert.Equal("LEE", stands[rank++].Team.Id);
            Assert.Equal("EVE", stands[rank++].Team.Id);
            Assert.Equal("AVL", stands[rank++].Team.Id);
            Assert.Equal("NEW", stands[rank++].Team.Id);
            Assert.Equal("WOL", stands[rank++].Team.Id);
            Assert.Equal("CRY", stands[rank++].Team.Id);
            Assert.Equal("SOU", stands[rank++].Team.Id);
            Assert.Equal("BHA", stands[rank++].Team.Id);
            Assert.Equal("BUR", stands[rank++].Team.Id);
            Assert.Equal("FUL", stands[rank++].Team.Id);
            Assert.Equal("WBA", stands[rank++].Team.Id);
            Assert.Equal("SHU", stands[rank++].Team.Id);

            rank = 0;
            Assert.Equal(1, stands[rank++].Rank);
            Assert.Equal(2, stands[rank++].Rank);
            Assert.Equal(3, stands[rank++].Rank);
            Assert.Equal(4, stands[rank++].Rank);
            Assert.Equal(5, stands[rank++].Rank);
            Assert.Equal(6, stands[rank++].Rank);
            Assert.Equal(7, stands[rank++].Rank);
            Assert.Equal(8, stands[rank++].Rank);
            Assert.Equal(9, stands[rank++].Rank);
            Assert.Equal(10, stands[rank++].Rank);
            Assert.Equal(11, stands[rank++].Rank);
            Assert.Equal(12, stands[rank++].Rank);
            Assert.Equal(13, stands[rank++].Rank);
            Assert.Equal(14, stands[rank++].Rank);
            Assert.Equal(15, stands[rank++].Rank);
            Assert.Equal(16, stands[rank++].Rank);
            Assert.Equal(17, stands[rank++].Rank);
            Assert.Equal(18, stands[rank++].Rank);
            Assert.Equal(19, stands[rank++].Rank);
            Assert.Equal(20, stands[rank++].Rank);
            #endregion

            #region Point 승점
            rank = 0;
            Assert.Equal(86, stands[rank++].Point);
            Assert.Equal(74, stands[rank++].Point);
            Assert.Equal(69, stands[rank++].Point);
            Assert.Equal(67, stands[rank++].Point);
            Assert.Equal(66, stands[rank++].Point);
            Assert.Equal(65, stands[rank++].Point);
            Assert.Equal(62, stands[rank++].Point);
            Assert.Equal(61, stands[rank++].Point);
            Assert.Equal(59, stands[rank++].Point);
            Assert.Equal(59, stands[rank++].Point);
            Assert.Equal(55, stands[rank++].Point);
            Assert.Equal(45, stands[rank++].Point);
            Assert.Equal(45, stands[rank++].Point);
            Assert.Equal(44, stands[rank++].Point);
            Assert.Equal(43, stands[rank++].Point);
            Assert.Equal(41, stands[rank++].Point);
            Assert.Equal(39, stands[rank++].Point);
            Assert.Equal(28, stands[rank++].Point);
            Assert.Equal(26, stands[rank++].Point);
            Assert.Equal(23, stands[rank++].Point);
            #endregion
        }
    }

    public static class LeagueExtensions
    {
        public static Team FindTeam(this League league, string id)
        {
            return league.Teams.FirstOrDefault(x => x.Id == id);
        }

        public static void AddMatch(this League league, string homeId, string awayId, int homeScore, int awayScore)
        {
            league.Matches.Add(new Match
            {
                Status = MatchStatus.Done,
                HomeTeam = league.FindTeam(homeId),
                AwayTeam = league.FindTeam(awayId),
                HomeScore = homeScore,
                AwayScore = awayScore,
            });
        }
    }
}