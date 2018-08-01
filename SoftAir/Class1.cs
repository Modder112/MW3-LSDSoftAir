using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityScript;

namespace SoftAir
{
    public class SoftAir : BaseScript
    {

        public struct SoftAirPlayer
        {
            public Entity player;
            public bool die;
            public bool isplayer;
        }

        public List<SoftAirPlayer> SAPlayers = new List<SoftAirPlayer>();
        public bool nextroundstarted = false;
        public int anzahl = 1;
        public SoftAir() : base ()
        {
            base.PlayerConnected += connect;
            Log.Write(LogLevel.All, "1:" + DescribeMapName(getDvar<string>("mapname")));
            Log.Write(LogLevel.All, "2:" + DescribeMapName(getDvar<string>("nextmap").Replace(" Isnipe_tdm", "").Replace(" Isnipe_tdm_as", "").Replace(" ","")));
        }

        public void connect(Entity player)
        {
            Utilities.RawSayTo(player,"^2Runde " + anzahl + " ist im gange!");
            AddPlayer(player);
            player.SpawnedPlayer += new Action(() =>
                {
                    SpawnPlayer(player);
                });
        }

        public void SpawnPlayer(Entity player)
        {
            SoftAirPlayer SAP = FindPlayerByEntity(player);
            if (SAP.die == true)
            {
                Utilities.RawSayTo(player, "^1Du bist getroffen!");
                player.TakeAllWeapons();
                player.Call("hide");
            }
            else
            {
                string uteam = player.GetField<string>("sessionteam");
                if (uteam.Equals("allies"))
                {
                    Vector3 SpawnPoint = new Vector3(1420.326f, 2233.146f, -254.875f);
                    player.Call("setorigin", new Parameter[]
						{
							new Parameter(SpawnPoint)
						});
                }
                else
                {
                    Vector3 SpawnPoint = new Vector3(-307.9711f, -15.88384f, -394.5965f);
                    player.Call("setorigin", new Parameter[]
						{
							new Parameter(SpawnPoint)
						});
                }
            }

        }

        public void reset()
        {
            anzahl = anzahl + 1;
            if (anzahl == 11)
            {

                Utilities.RawSayAll("^1Rund Endet! ^2Nächste Runde: " + DescribeMapName(getDvar<string>("nextmap").Replace(" Isnipe_tdm", "").Replace(" Isnipe_tdm_as", "").Replace(" ", "")));
                int waitt = 0;
                OnInterval(10000, () =>
                {
                    if (waitt == 0)
                    {
                        waitt++;
                        return true;
                    }
                    else
                    {
                        Utilities.ExecuteCommand("map_rotate");
                        return false;
                    }
                });
                
            }
            int wait = 0;
            OnInterval(10000, () =>
                {
                    if (wait == 0)
                    {
                        wait++;
                        return true;
                    }
                    else
                    {
                        Utilities.RawSayAll("^2Runde " + anzahl + " beginnt!");
                        foreach (Entity player in Players)
                        {
                            SoftAirPlayer SAP = FindPlayerByEntity(player);
                            SAP.die = false;
                            player.Call("show");
                            player.Call("suicide");
                            nextroundstarted = true;
                        }
                        return false;
                    }
                    
                });
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            player.Health += damage; 
            SoftAirPlayer SAP = FindPlayerByEntity(player);            
            if (mod == "MOD_FALLING")
            { }
            else
            {
                if (SAP.die == false)
                {
                    string uteam = player.GetField<string>("sessionteam");
                    if (uteam.Equals("allies"))
                    {
                        Utilities.SayAll("^2" + player.Name.ToString() + "^7[^0Lucky^1Strike^3Devils^7]", "^1HIT!!");
                    }
                    else
                    {
                        Utilities.SayAll("^2" + player.Name.ToString() + "^7[^3BadBoys^7]", "^1HIT!!");
                    }
                    player.TakeAllWeapons();
                    player.Call("hide");
                    SAP.die = true;
                }
            }
            bool areaplayerLSD = false;
            bool areaplayerBB = false;
            foreach (Entity p in Players)
            {
                string uteam = p.GetField<string>("sessionteam");
                if (uteam.Equals("allies"))
                {
                    if (SAP.die == false)
                    {
                        areaplayerLSD = true;
                    }
                }
                else
                {
                    if (SAP.die == false)
                    {
                        areaplayerBB = true;
                    }
                }
            }
            
            if (areaplayerLSD == true && areaplayerBB == false)
            {
                if (nextroundstarted == false)
                {
                    nextroundstarted = true;
                    Utilities.RawSayAll("^2Team ^0Lucky^1Strike^3Devils^2 hat gewonnen!");
                    reset();
                }
            }
            else if (areaplayerLSD == false && areaplayerBB == true)
            {
                if (nextroundstarted == false)
                {
                    nextroundstarted = true;
                    Utilities.RawSayAll("^2Team ^3BadBoys^2 hat gewonnen!");
                    reset();
                }
            }
            else if (areaplayerLSD == false && areaplayerBB == false)
            {
                if (nextroundstarted == false)
                {
                    nextroundstarted = true;
                    Utilities.RawSayAll("Unendschieden!");
                    reset();
                }
            }
        }

        public override EventEat OnSay3(Entity player, ChatType type, string name, ref string message)
        {
            if (message.Equals("!pos"))
            {
                Log.Write(LogLevel.All,"Player Pos: " + player.Origin.X.ToString() + ", " + player.Origin.Y.ToString() + ", " + player.Origin.Z.ToString() + "\n");
                return EventEat.EatGame;
            }
            else if (message.Equals("!team"))
            {
                string uteam = player.GetField<string>("sessionteam");
                Utilities.SayTo(player, "Server", uteam);
                return EventEat.EatGame;
            }
            else
            {
                string uteam = player.GetField<string>("sessionteam");
                if (uteam.Equals("allies"))
                {
                    Utilities.SayAll("^2" + player.Name.ToString() + "^7[^0Lucky^1Strike^3Devils^7]", message);
                    return EventEat.EatGame;
                }
                else
                {
                    Utilities.SayAll("^2" + player.Name.ToString() + "^7[^3BadBoys^7]", message);
                    return EventEat.EatGame;
                }
                
            }
        }

        public void AddPlayer(Entity player)
        {
            SoftAirPlayer SAP = new SoftAirPlayer();
            SAP.die = false;
            SAP.player = player;
            SAP.isplayer = true;
            SAPlayers.Add(SAP);
        }

        public void RemovePlayer(Entity player)
        {
            SAPlayers.Add(FindPlayerByEntity(player));            
        }

        public void CheckSAP(SoftAirPlayer SAP)
        {
            if(SAP.isplayer == true)
            {
                
            }
            else
            {
                SAPlayers.Clear();
                foreach (Entity p in Players)
                {
                    AddPlayer(p);
                    Utilities.RawSayAll("^1SERVER ERROR: ALL RESET!");
                    reset();
                }
            }
        }

        public SoftAirPlayer FindPlayerByEntity(Entity player)
        {
            SoftAirPlayer find = new SoftAirPlayer();
            find.isplayer = false;
            foreach (SoftAirPlayer SAP in SAPlayers)
            {
                if (SAP.player == player)
                {
                    find = SAP;
                }
            }
            CheckSAP(find);
            return find;
        }
        private T getDvar<T>(string dvar)
        {
            // would switch work here? - no
            if (typeof(T) == typeof(int))
            {
                return Call<T>("getdvarint", dvar);
            }
            else if (typeof(T) == typeof(float))
            {
                return Call<T>("getdvarfloat", dvar);
            }
            else if (typeof(T) == typeof(string))
            {
                return Call<T>("getdvar", dvar);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                return Call<T>("getdvarvector", dvar);
            }
            else
            {
                return default(T);
            }
        }

        public static string DescribeMapName(string map)
        {
            switch (map)
            {
                case "mp_afghan":
                    return "Afghan";
                case "mp_complex":
                    return "Bailout";
                case "mp_abandon":
                    return "Carnival";
                case "mp_crash":
                    return "Crash";
                case "mp_derail":
                    return "Derail";
                case "mp_estate":
                    return "Estate";
                case "mp_favela":
                    return "Favela";
                case "mp_fuel2":
                    return "Fuel";
                case "mp_highrise":
                    return "Highrise";
                case "mp_invasion":
                    return "Invasion";
                case "mp_checkpoint":
                    return "Karachi";
                case "mp_overgrown":
                    return "Overgrown";
                case "mp_quarry":
                    return "Quarry";
                case "mp_rundown":
                    return "Rundown";
                case "mp_rust":
                    return "Rust";
                case "mp_compact":
                    return "Salvage";
                case "mp_boneyard":
                    return "Scrapyard";
                case "mp_nightshift":
                    return "Skidrow";
                case "mp_storm":
                    return "Storm";
                case "mp_strike":
                    return "Strike";
                case "mp_subbase":
                    return "Sub Base";
                case "mp_terminal":
                    return "Terminal";
                case "mp_trailerpark":
                    return "Trailer Park";
                case "mp_underpass":
                    return "Underpass";
                case "mp_vacant":
                    return "Vacant";
                case "mp_brecourt":
                    return "Wasteland";
                case "contingency":
                    return "Contingency";
                case "oilrig":
                    return "Oilrig";
                case "invasion":
                    return "Burger Town";
                case "gulag":
                    return "Gulag";
                case "so_ghillies":
                    return "Pripyat";
                case "roadkill":
                    return "Roadkill";
                case "iw4_credits":
                    return "IW4 Test Map";
                case "trainer":
                    return "Trainer";
                case "dc_whitehouse":
                    return "White House";
                case "favela":
                    return "SpecOps Favela";
                case "mp_alpha":
                    return "Lockdown";
                case "mp_bootleg":
                    return "Bootleg";
                case "mp_bravo":
                    return "Mission";
                case "mp_carbon":
                    return "Carbon";
                case "mp_dome":
                    return "Dome";
                case "mp_exchange":
                    return "Downturn";
                case "mp_hardhat":
                    return "Hardhat";
                case "mp_interchange":
                    return "Interchange";
                case "mp_lambeth":
                    return "Fallen";
                case "mp_mogadishu":
                    return "Bakaara";
                case "mp_paris":
                    return "Resistance";
                case "mp_plaza2":
                    return "Arkaden";
                case "mp_radar":
                    return "Outpost";
                case "mp_seatown":
                    return "Seatown";
                case "mp_underground":
                    return "Underground";
                case "mp_village":
                    return "Village";
                case "mp_overwatch":
                    return "Overwatch";
                case "mp_park":
                    return "Liberation";
                case "mp_italy":
                    return "Piazza";
                case "mp_morningwood":
                    return "Black Box";
                case "mp_meteora":
                    return "Sanctuary";
                case "mp_foundation":
                    return "Foundation";
                case "mp_qadeem":
                    return "Oasis";
                case "mp_shipbreaker":
                    return "Decommission";
                case "mp_offshore":
                    return "Off Shore";
                case "mp_gulch":
                    return "Gulch";
                case "mp_boardwalk":
                    return "Boardwalk";
                case "mp_nola":
                    return "Parish";
                case "mp_crosswalk_ss ":
                    return "Intersection";
                case "mp_terminal_cls":
                    return "Terminal";
                case "mp_cement":
                    return "Foundation";
                case "mp_six_ss":
                    return "Vortex";
                case "mp_burn_ss":
                    return "U-turn";
                case "mp_courtyard_ss":
                    return "Erosion";
                case "mp_restrepo_ss":
                    return "Lookout";
                case "mp_crosswalk_ss":
                    return "Intersection";
                case "mp_aground_ss":
                    return "Aground";
                case "mp_hillside_ss":
                    return "Getaway";
                case "mp_moab":
                    return "Gulch";
                default:
                    return "^1" + map;
            }
        }

    }
}
