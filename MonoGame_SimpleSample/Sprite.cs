using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame_SimpleSample
{
    struct Triangle
    {
        public Vector2 A, B, C;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    class Sprite
    {
        public Boolean shouldDraw = true;
        protected Texture2D texture;
        public Vector2 position;
        public SpriteFont font;
        protected float rotation;
        protected SpriteEffects effects;

        private Texture2D rect;
        Vector2 origin;
        private List<Triangle> initialCollisionTriangles = new List<Triangle>();
        private List<Triangle> collisionTriangles= new List<Triangle>();


        protected int frameWidth;
        protected int frameHeight;

        public Sprite(Texture2D texture, Vector2 startingPosition, float rotation, List<Triangle> initialCollisionTriangles)
        {
            this.initialCollisionTriangles = initialCollisionTriangles;
            this.rotation = (float) (Math.PI * rotation / 180.0);
            position = startingPosition;
            this.texture = texture;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
        }
        public Sprite(Texture2D texture, Vector2 startingPosition, float rotation)
        {
            this.rotation = (float) (Math.PI * rotation / 180.0);
            position = startingPosition;
            this.texture = texture;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            
            
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            
            var aa = RotateVector(Vector2.Zero,  origin, rotation);
            var bb = RotateVector( new Vector2(texture.Width, 0),  origin, rotation);
            var cc = RotateVector( new Vector2(0, texture.Height),  origin, rotation);
            var dd = RotateVector( new Vector2(texture.Width, texture.Height),  origin, rotation);
            
            initialCollisionTriangles = new List<Triangle>();
            initialCollisionTriangles.Add(new Triangle(aa, bb, cc));
            initialCollisionTriangles.Add(new Triangle(bb, cc, dd));
        }

        public Sprite(Texture2D texture, Vector2 startingPosition) : this(texture, startingPosition, 0f)
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!shouldDraw) return;
        }


        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            if (!shouldDraw) return;
            if (initialCollisionTriangles != null)
            {
                var temp = new List<Triangle>();
                foreach (var triangle in initialCollisionTriangles)
                {
                    var aaa = RotateVector(triangle.A + position, position + origin, rotation);
                    var bbb = RotateVector(triangle.B + position, position + origin, rotation);
                    var ccc = RotateVector(triangle.C + position, position + origin, rotation);
                    temp.Add(new Triangle(aaa, bbb, ccc));
                }

                collisionTriangles = new List<Triangle>(temp);
            }

            spriteBatch.Draw(texture,
                new Rectangle((int) (position.X + origin.X), (int) (position.Y + origin.Y), frameWidth, frameHeight),
                null, Color.White, rotation, origin, effects, 0f);
            Debug_DrawBounds(graphicsDevice, spriteBatch);
           // if (font != null)
           //     spriteBatch.DrawString(font, position.ToString(), position + origin, Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
//(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

        public bool IsCollidingWith(Sprite otherSprite)
        {
            //;//
            //return this.boundingBox.Intersects(otherSprite.BoundingBox);
            return isIntersectingWith(otherSprite);
        }


        public void Debug_DrawBounds(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            if (rect == null)
                rect = new Texture2D(graphicsDevice, 1, 1);
            rect.SetData(new[] {Color.White});
            if(initialCollisionTriangles != null)
            foreach (var triangle in collisionTriangles)
            {
                DrawTriangle(spriteBatch,triangle.A, triangle.B, triangle.C, Color.Aqua);
            }
        }

        static Vector2 RotateVector(Vector2 pointToRotate, Vector2 centerPoint, double angleInRadians)
        {
            var ry = RotatePoint(new Point((int) pointToRotate.X, (int) pointToRotate.Y),
                new Point((int) (centerPoint.X), (int) (centerPoint.Y)), RadianToDegree(angleInRadians));
            var lool = new Vector2((float) ry.X, (float) ry.Y);
            return lool;
        }

        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                     sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                     cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        private void DrawRectangle(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, BoundingBox boundingBox,
            Color color)
        {
            int rectWidth = (int) (boundingBox.Max.X - boundingBox.Min.X);
            int rectHeight = (int) (boundingBox.Max.Y - boundingBox.Min.Y);

            Rectangle coords = new Rectangle((int) boundingBox.Min.X, (int) boundingBox.Min.Y, rectWidth, rectHeight);

            //DrawLine(spriteBatch, position, position + new Vector2(texture.Width, texture.Height), Color.Aqua);
            // DrawLine(spriteBatch, position, position + new Vector2(texture.Width, texture.Height), Color.LimeGreen);
            //spriteBatch.Draw(rect, new Rectangle((int)boundingBox.Min.X, (int)boundingBox.Min.Y, 2, 2), Color.Aqua);
            //spriteBatch.Draw(rect, new Rectangle((int)boundingBox.Min.X+ rectWidth - 2, (int)boundingBox.Min.Y+ rectHeight - 2, 2, 2), Color.Aqua);
            spriteBatch.Draw(rect, coords, color);
        }

        public bool isIntersectingWith(Sprite sprite)
        {
            if (texture.Name == sprite.texture.Name || initialCollisionTriangles == null)
                return false;


            for (var i = 0; i < collisionTriangles.Count; i++)
            {
            //foreach (var triangle in collisionTriangles)
            //{
                var triangle = collisionTriangles[i];
                for (var j = 0; j < sprite.collisionTriangles.Count; j++)
                //foreach (var triangleOther in sprite.collisionTriangles)
                {
                    var triangleOther = sprite.collisionTriangles[j];
                    var aa = triangle.A;//RotateVector(position, position + origin, rotation);
                    var bb = triangle.B;//RotateVector(position + new Vector2(texture.Width, 0), position + origin, rotation);
                    var cc = triangle.C;//RotateVector(position + new Vector2(0, texture.Height), position + origin, rotation);
                    var a = triangleOther
                        .A; //RotateVector(sprite.position, sprite.position + origin2, sprite.rotation);
                    var b = triangleOther
                        .B; //RotateVector(sprite.position + new Vector2(sprite.texture.Width, 0), sprite.position + origin2, sprite.rotation);
                    var c = triangleOther
                        .C; //RotateVector(sprite.position + new Vector2(0, sprite.texture.Height), sprite.position + origin2, sprite.rotation);

                    var tri1Area = Math.Abs((a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) / 2);
                    var seg1Area = Math.Abs((a.X * (b.Y - aa.Y) + b.X * (aa.Y - a.Y) + aa.X * (a.Y - b.Y)) / 2);
                    var seg2Area = Math.Abs((a.X * (aa.Y - c.Y) + aa.X * (c.Y - a.Y) + c.X * (a.Y - aa.Y)) / 2);
                    var seg3Area = Math.Abs((aa.X * (b.Y - c.Y) + b.X * (c.Y - aa.Y) + c.X * (aa.Y - b.Y)) / 2);
                    //  var a1 = RotateVector(position + new Vector2(texture.Width, 0), position + origin, rotation);
                    //  var a2= RotateVector(position + new Vector2(texture.Width, texture.Height), position + origin, rotation);
                    //   var a3= RotateVector(position + new Vector2(0, texture.Height), position + origin, rotation);

                    var seg1Area2 = Math.Abs((a.X * (b.Y - bb.Y) + b.X * (bb.Y - a.Y) + bb.X * (a.Y - b.Y)) / 2);
                    var seg2Area2 = Math.Abs((a.X * (bb.Y - c.Y) + bb.X * (c.Y - a.Y) + c.X * (a.Y - bb.Y)) / 2);
                    var seg3Area2 = Math.Abs((bb.X * (b.Y - c.Y) + b.X * (c.Y - bb.Y) + c.X * (bb.Y - b.Y)) / 2);

                    var seg1Area3 = Math.Abs((a.X * (b.Y - cc.Y) + b.X * (cc.Y - a.Y) + cc.X * (a.Y - b.Y)) / 2);
                    var seg2Area3 = Math.Abs((a.X * (cc.Y - c.Y) + cc.X * (c.Y - a.Y) + c.X * (a.Y - cc.Y)) / 2);
                    var seg3Area3 = Math.Abs((cc.X * (b.Y - c.Y) + b.X * (c.Y - cc.Y) + c.X * (cc.Y - b.Y)) / 2);

                    var res = Math.Abs(tri1Area - (seg1Area + seg2Area + seg3Area)) < 5f ||
                              Math.Abs(tri1Area - (seg1Area2 + seg2Area2 + seg3Area2)) < 5f ||
                              Math.Abs(tri1Area - (seg1Area3 + seg2Area3 + seg3Area3)) < 5f;
                      Console.WriteLine("DEBUG:" + this.texture.Name + ", other: " + sprite.texture.Name);
                      Console.WriteLine("a" + a);
                      Console.WriteLine("b" + b);
                      Console.WriteLine("c" + c);
                      Console.WriteLine("aa" + aa);
                      Console.WriteLine("bb" + bb);
                      Console.WriteLine("cc" + cc);
                      Console.WriteLine("res" + res);
                      Console.WriteLine("(seg1Area + seg2Area + seg3Area)" + (seg1Area + seg2Area + seg3Area));
                      Console.WriteLine("tri1Area" + tri1Area);
                    if (res) return true;

                }
            }

            return false;
        }

        public String toLevelFormat()
        {
            return texture.Name + ";" + position.X + ";" + position.Y + ";" + RadianToDegree(rotation);
        }

        static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        void DrawTriangle(SpriteBatch sb, Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            DrawLine(sb, a, b, color);
            DrawLine(sb, b, c, color);
            DrawLine(sb, c, a, color);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float) Math.Atan2(edge.Y, edge.X);


            sb.Draw(rect,
                new Rectangle( // rectangle defines shape of line and position of start of line
                    (int) start.X,
                    (int) start.Y,
                    (int) edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle, //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }
}