using Microsoft.Xna.Framework;
using System;

public class RotatedRectangle
{
    public Rectangle Rectangle { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }

    public RotatedRectangle(Rectangle rect, float rotation)
    {
        Rectangle = rect;
        Rotation = rotation;
        Origin = new Vector2(rect.Width / 2f, rect.Height / 2f);
    }

    // Get the four corners of the rotated rectangle
    public Vector2[] GetCorners()
    {
        Vector2[] corners = new Vector2[4];
        Vector2 center = new Vector2(Rectangle.X + Rectangle.Width / 2f, Rectangle.Y + Rectangle.Height / 2f);

        corners[0] = new Vector2(-Rectangle.Width / 2f, -Rectangle.Height / 2f);
        corners[1] = new Vector2(Rectangle.Width / 2f, -Rectangle.Height / 2f);
        corners[2] = new Vector2(Rectangle.Width / 2f, Rectangle.Height / 2f);
        corners[3] = new Vector2(-Rectangle.Width / 2f, Rectangle.Height / 2f);

        // Rotate each corner
        for (int i = 0; i < 4; i++)
        {
            float x = corners[i].X * (float)Math.Cos(Rotation) - corners[i].Y * (float)Math.Sin(Rotation);
            float y = corners[i].X * (float)Math.Sin(Rotation) + corners[i].Y * (float)Math.Cos(Rotation);
            corners[i] = new Vector2(x + center.X, y + center.Y);
        }

        return corners;
    }

    // Separating Axis Theorem collision detection
    public bool Intersects(RotatedRectangle other)
    {
        Vector2[] corners1 = GetCorners();
        Vector2[] corners2 = other.GetCorners();

        // Check both rectangles' axes
        for (int shape = 0; shape < 2; shape++)
        {
            Vector2[] corners = shape == 0 ? corners1 : corners2;

            for (int i = 0; i < 4; i++)
            {
                Vector2 axis = corners[(i + 1) % 4] - corners[i];
                axis = new Vector2(-axis.Y, axis.X); // perpendicular
                if (axis != Vector2.Zero)
                    axis.Normalize();

                float min1 = float.MaxValue, max1 = float.MinValue;
                float min2 = float.MaxValue, max2 = float.MinValue;

                // Project corners1 onto axis
                foreach (var corner in corners1)
                {
                    float projection = Vector2.Dot(corner, axis);
                    min1 = Math.Min(min1, projection);
                    max1 = Math.Max(max1, projection);
                }

                // Project corners2 onto axis
                foreach (var corner in corners2)
                {
                    float projection = Vector2.Dot(corner, axis);
                    min2 = Math.Min(min2, projection);
                    max2 = Math.Max(max2, projection);
                }

                // Check for gap
                if (max1 < min2 || max2 < min1)
                    return false;
            }
        }

        return true;
    }

    // Simple collision with non-rotated rectangle
    public bool Intersects(Rectangle rect)
    {
        return Intersects(new RotatedRectangle(rect, 0));
    }
}
