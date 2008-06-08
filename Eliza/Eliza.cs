using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace Eliza
{
    public class ElizaApi
    {
        const string ElizaUrl = "eliza";
        const string MapUrl = "maplayout/";
        const string GameUrl = "gamestate/";
        const string AcceptInvitation = "acceptInvitation";
        const string DeclineInvitation = "declineInvitation";
        const string SendReminder = "sendReminder";
        const string Surrender = "surrender";
        const string Abandon = "abandon";
        const string FinishTurn = "finishTurn";
        const string RemoveGame = "removeGame";

        string _username;
        string _token;
        string _urlString;
        string _lastResult;

        public ElizaApi(String ur, String u, String t)
        {
            _username = u;
            _urlString = ur;
            _token = t;
        }

        public string User { get { return _username; } }
        private XmlDocument GetDocument(string uri, string user, string password, string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(DoRequest(uri,user,password,xml)));
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            return doc;
        }

        private String DoRequest(String u, String username, String password, String xml)
        {
            WebRequest request = WebRequest.Create(u);
            if( username!=null )
            {
                String userPassword = username + ":" + password;
                String encoding = Convert.ToBase64String(Encoding.Default.GetBytes(userPassword));
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + encoding);
            }
            if( xml!=null )
            {
                request.Method = "POST";
                request.ContentType = "application/xml";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write(xml);
                writer.Close();
            }
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                StringBuilder sb = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    sb.Append(reader.ReadLine());
                }
                _lastResult = sb.ToString();
                return _lastResult;
            }
            catch (WebException) { return null; }
        }
        public string GetLastResult()
        {
            return _lastResult;
        }

        private bool DoGameAction(int id, string action)
        {
            String result = DoRequest(_urlString + ElizaUrl, _username, _token, "<weewar game=\"" + id + "\"><" + action + "/></weewar>");
            return result.Contains("<ok/>");
        }

        public bool AcceptInvite(int id)
        {
            return DoGameAction(id, AcceptInvitation);
        }

        public bool DeclineInvite(int id)
        {
            return DoGameAction(id, DeclineInvitation);
        }

        public Game GetGameState(int id)
        {
            return parseGame(GetDocument(_urlString + GameUrl + id, _username, _token, "<weewar game=\"" + id + "\"><acceptInvitation/></weewar>").DocumentElement);
        }

        public List<Game> GetGamesFromHeadquarters()
        {
            XmlDocument doc = GetDocument(_urlString + "headquarters", _username, _token, null);
            List<Game> ret = new List<Game>();
            //System.out.println(" Doc: "+doc.RootElement.getChildren("game") );
            XmlNodeList games = doc.SelectNodes("/games/game");
            foreach (XmlNode gameEle in games)
            {
                Game g = parseGame(gameEle as XmlElement);
                ret.Add(g);
            }
            return ret;
        }

        private Game parseGame(XmlElement gameEle)
	{
		Game g = new Game();
		String att = gameEle.GetAttribute("inNeedOfAttention");
        g.IsInNeedOfAttention = att == "true";
		g.Name =  gameEle.SelectSingleNode("name").InnerText ;
		g.Id =  Int32.Parse( gameEle.SelectSingleNode("id").InnerText ) ;
        XmlNode map = gameEle.SelectSingleNode("map");

		if( map != null && map.InnerText!=null )
			g.MapId =  Int32.Parse( map.InnerText ) ;
		g.Link =  gameEle.SelectSingleNode("url").InnerText ;
		g.RequiresAnInviteAccept =  g.Link.Contains( "join" ) ;
		if( gameEle.SelectSingleNode("factions")!=null  )
		{
            XmlNodeList factions = gameEle.SelectNodes("factions/faction");
			foreach ( XmlElement faction in factions)
			{
				Faction f =  new Faction();
				g.Factions.Add( f );
				f.PlayerName =  faction.GetAttribute("playerName") ;
				f.State =  faction.GetAttribute("state") ;
				if(  !String.IsNullOrEmpty(faction.GetAttribute("credits")))
					f.Credits = Int32.Parse( faction.GetAttribute("credits") ) ;
				foreach ( XmlElement unit in faction.SelectNodes("unit"))
				{
					Unit u = new Unit();
					u.Coordinate =  new Coordinate( Int32.Parse( unit.GetAttribute("x") ), Int32.Parse( unit.GetAttribute("y") ) ) ;
					u.Type =  Unit.ToType(unit.GetAttribute("type")) ;
					u.Finished =  "true" == unit.GetAttribute("finished") ;
					u.Quantity =   Int32.Parse( unit.GetAttribute("quantity") ) ;
					f.Units.Add( u );
				}
				foreach ( XmlElement unit in faction.SelectNodes("terrain"))
				{
					Terrain t = new Terrain();
					t.Coordinate =  new Coordinate( Int32.Parse( unit.GetAttribute("x") ), Int32.Parse( unit.GetAttribute("y") ) ) ;
					t.Type =   unit.GetAttribute("type");
                    t.Finished = "true" == unit.GetAttribute("finished");
					f.Terrains.Add( t );
				}
			}
		}
		return g;
	}
        private WeewarMap parseMap(XmlElement mapEle)
        {
            WeewarMap wmap = new WeewarMap();
            wmap.Width = Int32.Parse(mapEle.SelectSingleNode("width").InnerText);
            wmap.Height = Int32.Parse(mapEle.SelectSingleNode("height").InnerText);
            XmlNodeList terrains = mapEle.SelectNodes("terrains/terrain");
            foreach (XmlElement terrain in terrains)
            {
                Terrain t = new Terrain();
                t.Coordinate = new Coordinate(Int32.Parse(terrain.GetAttribute("x")), Int32.Parse(terrain.GetAttribute("y")));
                t.Type = terrain.GetAttribute("type");
                wmap.add(t);
            }
            return wmap;
        }

        public List<Coordinate> GetMovementCoords(int id, Coordinate from, UnitType unitType)
        {
            string type = Unit.ToString(unitType);
            String requestXml = "<weewar game='" + id + "'><movementOptions x='" + from.X + "' y='" + from.Y + "' type='" + type + "' /></weewar>";
            XmlDocument doc = GetDocument(_urlString + "eliza", _username, _token, requestXml);
            List<Coordinate> coords = new List<Coordinate>();
            foreach (XmlElement coord in doc.SelectNodes("//coordinate"))
            {
                Coordinate c = new Coordinate(Int32.Parse(coord.GetAttribute("x")), Int32.Parse(coord.GetAttribute("y")));
                coords.Add(c);
            }
            return coords;
        }

        public String MoveAttackCapture(int id, Coordinate from, Coordinate to, Coordinate attack, bool capture)
        {
            String attackString = "";
            if (attack != null)
                attackString = "<attack x='" + attack.X + "' y='" + attack.Y + "' />";

            String moveString = "";
            if (from != to)
                moveString = "<move x='" + to.X + "' y='" + to.Y + "' />";
            String captureString = capture ? "<capture/>" : "";

            String requestXml = "<weewar game='" + id + "'><unit x='" + from.X + "' y='" + from.Y + "' >" + moveString + captureString + attackString + "</unit></weewar>";
            String xml = DoRequest(_urlString + ElizaUrl, _username, _token, requestXml);
            return xml;
        }

        public bool EndTurn(int id)
        {
            return DoGameAction(id, FinishTurn);
        }

        public String Build(int id, Coordinate c, UnitType unitType)
        {
            string type = Unit.ToString(unitType);
            String requestXml = "<weewar game='" + id + "'><build  x='" + c.X + "' y='" + c.Y + "' type='" + type + "' /></weewar>";
            String xml = DoRequest(_urlString + ElizaUrl, _username, _token, requestXml);
            return xml;
        }

        public List<Coordinate> GetAttackCoords(int id, Coordinate from, UnitType unitType)
        {
            string type = Unit.ToString(unitType);
            String requestXml = "<weewar game='" + id + "'><attackOptions x='" + from.X + "' y='" + from.Y + "' type='" + type + "' /></weewar>";
            XmlDocument doc = GetDocument(_urlString + ElizaUrl, _username, _token, requestXml);
            List<Coordinate> coords = new List<Coordinate>();
            foreach (XmlElement coord in doc.SelectNodes("//coordinate"))
            {
                Coordinate c = new Coordinate(Int32.Parse(coord.GetAttribute("x")), Int32.Parse(coord.GetAttribute("y")));
                coords.Add(c);
            }
            return coords;
        }


        public WeewarMap GetMap(int mapId)
        {
            String xml = DoRequest(_urlString + MapUrl + mapId, _username, _token, null);
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            return parseMap(doc.DocumentElement);
        }

        public String Repair(int id, Coordinate coordinate)
        {
            String requestXml = "<weewar game='" + id + "'><unit x='" + coordinate.X + "' y='" + coordinate.Y + "' ><repair/></unit></weewar>";
            String xml = DoRequest(_urlString + ElizaUrl, _username, _token, requestXml);
            return xml;

        }

    }
}
