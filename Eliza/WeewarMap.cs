using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;

namespace Eliza
{
  public class WeewarMap
  {
    Dictionary<Coordinate, Terrain> terrains = new Dictionary<Coordinate, Terrain>();
    int width, height;

    public WeewarMap(XmlElement el)
    {
      Width = Int32.Parse(el.SelectSingleNode("width").InnerText);
      Height = Int32.Parse(el.SelectSingleNode("height").InnerText);
      XmlNodeList terrains = el.SelectNodes("terrains/terrain");
      foreach (XmlElement terrain in terrains)
      {
        Terrain t = new Terrain();
        t.Coordinate = new Coordinate(Int32.Parse(terrain.GetAttribute("x")), Int32.Parse(terrain.GetAttribute("y")));
        t.Type = Terrain.ToType(terrain.GetAttribute("type"));
        add(t);
      }
    }

    public int Width
    {
      get { return width; }
      set { width = value; }
    }

    public int Height
    {
      get { return height; }
      set { height = value; }
    }

    public ICollection<Terrain> Terrains
    {
      get { return terrains.Values; }
    }



    public void add(Terrain t)
    {
      terrains.Add(t.Coordinate, t);
    }

    public Terrain get(int x, int y)
    {
      return get(new Coordinate(x, y));
    }

    public Terrain get(Coordinate c)
    {
      return terrains[c];
    }


    public List<Terrain> getTerrainsByType(TerrainType type)
    {
      List<Terrain> terrains = new List<Terrain>();
      foreach (Terrain t in Terrains)
        if (t.Type == type)
          terrains.Add(t);
      return terrains;
    }


    public bool ContainsKey(Coordinate coord)
    {
      return terrains.ContainsKey(coord);
    }
  }
}
