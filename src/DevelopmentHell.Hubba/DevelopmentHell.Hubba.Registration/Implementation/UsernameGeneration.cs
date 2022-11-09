using Azure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Registration.Implementation
{
    public class UsernameGeneration
    {
        private string[] animals = {"Cat", "Dog", "Elephant", "Serval", "Ocelot", "Giraffe", "Caracal", "Saiga", "Viper", "Macaw", "Bear", "Margay",
            "Ferret", "Tapir", "Agouti", "Fringehead", "Markhor", "Kookaburra", "Kingfisher", "Langur", "Baboon", "Mandrill",
            "Vulture", "Buzzard", "Avocet", "Albatross", "Dragon", "Lemur", "Sifaka", "Cockroach", "Fossa", "Otter", "Raccoon", "Quail", "Emu",
            "Rhea", "Cassowary", "Pademelon", "Barracuda", "Owl", "Alpaca", "Paca", "Gemsbok", "Eel", "Cheetah", "Catfish",
            "Pangolin", "Armadillo", "Eagle", "Galago", "Crab", "Bat", "Walrus", "Beetle", "Fox", "Addax", "Caracara", "Duiker", "Eider", "Gar", "Seahorse",
            "Kangaroo", "Wallaby", "Chameleon", "Snake", "Binturong", "Gerbil", "Hamster", "Pig", "Manatee", "Lion", "Seal", "Squid", "Cuttlefish", "Cod",
            "Lobster", "Crab", "Orca", "Civet", "Barb", "Dugong", "Cow", "Woodpecker", "Wombat", "Penguin", "Newt", "Toad", "Shark", "Okapi", "Fish",
            "Krait", "Boa" };

        private string[] adjectives = {"other", "new", "good", "high", "old", "great", "big", "American", "small", "large", "national",
            "young", "different", "little", "important", "political", "bad", "real", "best", "social", "only", "low", "early", "human", "local", "late",
            "hard", "major", "better", "economic", "strong", "whole", "free", "military", "true", "federal", "international", "full", "special",
            "easy", "clear", "recent", "certain", "personal", "open", "red", "difficult", "available", "likely", "short", "single", "medical", "current",
            "wrong", "private", "past", "foreign", "fine", "common", "poor", "natural", "significant", "similar", "hot", "central", "happy", "serious",
            "ready", "simple", "left", "physical", "general", "environmental", "financial", "blue", "democratic", "dark", "various", "entire", "close", "legal",
            "religious", "cold", "final", "main", "green", "nice", "huge", "popular", "traditional", "cultural"};

        private string username;

        public String generateUsername()
        {
            Random random = new Random();

            //adjective

            string adjective = adjectives[random.Next(adjectives.Length)];
            username += adjective.ToLower() + '.';

            //animal
            for (int i = 0; i < 2; i++)
            {
                string animal = animals[random.Next(animals.Length)];
                while (username != null && animal == username.Remove(username.Length - 1, 1))
                {
                    animal = animals[random.Next(animals.Length)];
                }
                username += animal.ToLower() + '.';
            }
            username = username.Remove(username.Length - 1, 1);


            return username;
        }


    }
}
