﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using DoctorWh0oDiscordBot.Models;
using System.Linq;

namespace DoctorWh0oDiscordBot.Core.Commands
{
    
    public class DndSpell : ModuleBase<SocketCommandContext>
    {
        const string UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";
        string SpellName;

        [Command("spells"), Alias("Spells", "spell", "Spell")]
        public async Task produceURL([Remainder]string message)
        {
            SpellName = message.ToLower();
            
            Dictionary<string, string> SpellDictionary = CreateArrayOfAllSpells();
            string url;
            if (SpellDictionary.TryGetValue(SpellName, out url))
            {
                int spellIndex = (Array.IndexOf(SpellDictionary.Keys.ToArray(), SpellName) + 1);

                SpellDetails spellDetials = ProduceSpellInfo(SpellName);
                EmbedBuilder SpellCard = SpellCardMaker(spellDetials);
                await Context.Channel.SendMessageAsync("", false, SpellCard.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync(TypoResponse.TypoRespond(SpellDictionary,SpellName, "spell"));
            }

        }

       


        public Dictionary<string, string> CreateArrayOfAllSpells()
        {
            using (var webClient = new WebClient())
            {
                //get string representation of JSON
                string rawJSON = webClient.DownloadString("http://dnd5eapi.co/api/spells");
                //convert string to RootObjects
                RootObject spellList = JsonConvert.DeserializeObject<RootObject>(rawJSON);
                var SpellDictionary = new Dictionary<string, string>();

                for (int i = 0; i < spellList.count; i++)
                {
                    SpellDictionary.Add(spellList.results[i].name.ToLower(), spellList.results[i].url);
                }
                return SpellDictionary;
            }
        }

        public SpellDetails ProduceSpellInfo(string spellName)
        {
            using (var webClient = new WebClient())
            {
                spellName = spellName.Replace(" ", "-");
                string rawJSON = webClient.DownloadString("http://dnd5eapi.co/api/spells/" + spellName);
                
                SpellDetails spellInfo = JsonConvert.DeserializeObject<SpellDetails>(rawJSON);

                return(spellInfo);
            }
            
        }

        public EmbedBuilder SpellCardMaker(SpellDetails spellDetails)
        {
            string components ="";
            foreach (string x in spellDetails.components)
            {
                components += x + " ";
            }

            var spellCard = new EmbedBuilder();
            spellCard.WithTitle(spellDetails.name);
            spellCard.WithColor(Color.Blue);
            if(!(spellDetails.casting_time == null)) spellCard.AddField("Casting Time", spellDetails.casting_time, true);
            if (!(spellDetails.range == null)) spellCard.AddField("Range", spellDetails.range, true);
            if (!(spellDetails.duration == null)) spellCard.AddField("Duration", spellDetails.duration, true);
            if (!(spellDetails.components == null)) spellCard.AddField("Components", components, true);
            if (!(spellDetails.material == null)) spellCard.AddField("Materials", spellDetails.material, true);
            spellCard.AddField("Spell level", spellDetails.level, true);
            if (!(spellDetails.desc[0] == null)) spellCard.WithDescription(spellDetails.desc[0]);
            if (!(spellDetails.concentration == null)) spellCard.AddField("Concentration", spellDetails.concentration, true);
            //if (!(spellDetails.higher_level[0] == null)) spellCard.AddField("At Higher Levels", spellDetails.higher_level[0], false);
            //if (spellDetails.higher_level != null) spellCard.AddField("At higher levels", spellDetails.higher_level[0], false);
            //ADD RITUAL, AOE, SCHOOL, CLASSES, SUBCLASSES
            return spellCard;
        }

    }
    
}

