using System;
using System.Collections.Generic;

using System.Text;

namespace Eliza
{
    public class Coordinate
    {
	    static List<Direction> dirs = new List<Direction>();
	    int x,y;
	    public enum Direction{
		    RANGED,
		    EAST,
		    NORTHEAST,
		    NORTHWEST,
		    WEST,
		    SOUTHWEST,
		    SOUTHEAST,
	    }

        static Coordinate()
        {
		    dirs.Add ( Direction.EAST );
		    dirs.Add( Direction.NORTHEAST );
		    dirs.Add( Direction.NORTHWEST );
            dirs.Add(Direction.SOUTHEAST);
            dirs.Add(Direction.SOUTHWEST);
            dirs.Add(Direction.WEST);
	    }


	    public Coordinate() {
	    }

	    public Coordinate( int x, int y )
	    {
		    this.x=x;
		    this.y=y;
	    }
	    public Coordinate( Coordinate c )
	    {
		    this.x=c.x;
		    this.y=c.y;
	    }
        public override int  GetHashCode()
        {
		    return x*1000+y;
	    }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
	    }

	    public Direction getDirection( Coordinate c ){
		    int dx = c.X-x;
		    int dy = c.Y-y;
		    if( dx<-1 || dx>1 || dy < -1 || dy > 1 )
			    return Direction.RANGED;
		    if( dy==0 && dx==1 )
			    return Direction.EAST;
		    if( dy==0 && dx==-1 )
			    return Direction.WEST;
		    if( y % 2==0 )
		    {
			    if( dy==-1 && dx==-1 )
				    return Direction.NORTHWEST;
			    if( dy==-1 && dx==0 )
				    return Direction.NORTHEAST;
			    if( dy==1 && dx==-1 )
				    return Direction.SOUTHWEST;
			    if( dy==1 && dx==0 )
				    return Direction.SOUTHEAST;
		    }
		    else
		    {
			    if( dy==-1 && dx==0 )
				    return Direction.NORTHWEST;
			    if( dy==-1 && dx==1 )
				    return Direction.NORTHEAST;
			    if( dy==1 && dx==0 )
				    return Direction.SOUTHWEST;
			    if( dy==1 && dx==1 )
				    return Direction.SOUTHEAST;
		    }
		    return Direction.RANGED;
	    }

	    public static Direction oppositeDirection( Direction dir )
	    {
		    if     ( dir==Direction.WEST  	 ) return Direction.EAST;
		    else if( dir==Direction.NORTHWEST ) return Direction.SOUTHEAST;
		    else if( dir==Direction.NORTHEAST ) return Direction.SOUTHWEST;
		    else if( dir==Direction.EAST  	 ) return Direction.WEST;
		    else if( dir==Direction.SOUTHEAST ) return Direction.NORTHWEST;
		    else if( dir==Direction.SOUTHWEST ) return Direction.NORTHEAST;
		    else
			    return Direction.RANGED;
	    }

	    public int getDistance( Coordinate c )
	    {

		    int yh = y%2;
		    int x1 =  (int)( x-Math.Ceiling( (Math.Abs( c.y-y )-yh)/2.0 ));
		    int x2 =  (int)( x+Math.Floor( (Math.Abs( c.y-y )+yh)/2.0 )) ;
		    if( x1 <= c.x && x2>=c.x )
		    {
			    return Math.Abs( c.y-y );
		    }
		    else
			    if( x1> c.x )
			    {
				    return Math.Abs( c.y-y )+Math.Abs( c.x-x1 );
			    }
			    else
			    {
				    return Math.Abs( c.y-y )+Math.Abs( c.x-x2 );
			    }

	    }
        public static bool operator !=(Coordinate lhs, Coordinate rhs)
        {
            return !(lhs == rhs);
        }
        public static bool operator== (Coordinate lhs, Coordinate rhs)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)lhs == null) || ((object)rhs == null))
            {
                return false;
            }

            // Return true if the fields match:
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }
	    /* (non-Javadoc)
	     * @see java.lang.Object#equals(java.lang.Object)
	     */
	    override public bool Equals(Object arg0) {
            Coordinate c = arg0 as Coordinate;
            return this == c;
	    }

	    override public String ToString() {
		    return "{\"x\":"+x+",\"y\":"+y+"}";
	    }


	    public Coordinate getCoordinateInDirection(Direction dir) {
		    int x = this.x;
		    int y = this.y;
		    if( y % 2==0 )
		    {
			    if     ( dir==Direction.EAST		 ) { x+=+1;y+= 0; }
			    else if( dir==Direction.WEST		 ) { x+=-1;y+= 0; }
			    else if( dir==Direction.NORTHEAST ) { x+= 0;y+=-1; }
			    else if( dir==Direction.NORTHWEST ) { x+=-1;y+=-1; }
			    else if( dir==Direction.SOUTHEAST ) { x+= 0;y+= 1; }
			    else if( dir==Direction.SOUTHWEST ) { x+=-1;y+= 1; }
		    }
		    else
		    {
			    if     ( dir==Direction.EAST		 ) { x+=+1;y+= 0; }
			    else if( dir==Direction.WEST		 ) { x+=-1;y+= 0; }
			    else if( dir==Direction.NORTHEAST ) { x+= 1;y+=-1; }
			    else if( dir==Direction.NORTHWEST ) { x+= 0;y+=-1; }
			    else if( dir==Direction.SOUTHEAST ) { x+= 1;y+= 1; }
			    else if( dir==Direction.SOUTHWEST ) { x+= 0;y+= 1; }

		    }
		    return new Coordinate( x, y );
	    }


	    public static Direction clockwise( Direction dir )
	    {
		    if     ( dir==Direction.EAST		 ) { return Direction.SOUTHEAST; }
		    else if( dir==Direction.WEST		 ) { return Direction.NORTHWEST; }
		    else if( dir==Direction.NORTHEAST ) { return Direction.EAST; }
		    else if( dir==Direction.NORTHWEST ) { return Direction.NORTHEAST; }
		    else if( dir==Direction.SOUTHEAST ) { return Direction.SOUTHWEST;  }
			    return Direction.WEST;
	    }

	    public static List<Direction> getAllDirections()
	    {
		    return dirs;
	    }

	    public List<Coordinate> getCircle( int radius )
	    {

		    List<Coordinate> l = new List<Coordinate>();
		    l.Add( this );
		    if( radius==0 )
			    return l;
		    for( int ix=X-radius-1; ix<X+radius+1; ix++ )
			    for( int iy=Y-radius-1; iy<Y+radius+1; iy++ )
			    {
				    Coordinate to = new Coordinate( ix,iy );
				    if( getDistance( to )<=radius )
					    l.Add( to );
			    }
		    return l;
	    }

        public static string ToString(IList<Coordinate> coords)
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < coords.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(coords[i].ToString());
            }
            sb.Append("]");
            return sb.ToString();

        }

        public Coordinate Xform(int xmod, int ymod)
        {
          return new Coordinate(x + xmod, y + ymod);
        }
      }
}
