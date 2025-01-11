using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_winforms_z03
{
    public class Object3D
    {
        public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
        public List<int[]> Faces { get; private set; } = new List<int[]>(); // Lista fețelor (quads)
        public Vector3 Position { get; private set; } = Vector3.Zero;
        public Vector3 Rotation { get; private set; } = Vector3.Zero;
        public Vector3 ScaleFactors { get; private set; } = Vector3.One;
        public Color Color { get; set; }

        public void Translate(float x, float y, float z)
        {
            Position += new Vector3(x, y, z);
        }

        public void Rotate(float angle, float x, float y, float z)
        {
            Rotation += new Vector3(angle * x, angle * y, angle * z);
        }

        public void Scale(float x, float y, float z)
        {
            ScaleFactors = new Vector3(ScaleFactors.X * x, ScaleFactors.Y * y, ScaleFactors.Z * z);
        }

        public void GenerateFaces()
        {
            // Generăm fețele cubului (quads)
            Faces.Add(new[] { 0, 1, 2, 3 }); // Fața 1
            Faces.Add(new[] { 4, 5, 6, 7 }); // Fața 2
            Faces.Add(new[] { 1, 5, 6, 2 }); // Fața 2
            Faces.Add(new[] { 0, 4, 7, 3 }); // Fața 2
            Faces.Add(new[] { 0, 4, 5, 1 }); // Fața 2
            Faces.Add(new[] { 3, 7, 6, 2 }); // Fața 2
            // ... continuăm pentru toate cele 6 fețe
        }
        public bool IsMouseOver(int mouseX, int mouseY, int screenWidth, int screenHeight, Vector3 cameraPosition)
        {
            foreach (var vertex in Vertices)
            {
                // Proiectăm fiecare vertex al obiectului în spațiul ecranului
                Vector4 clipSpace = Vector4.Transform(new Vector4(vertex, 1.0f), GetModelViewProjectionMatrix(cameraPosition));
                if (clipSpace.W == 0) continue; // Evităm divizarea cu zero

                Vector3 ndcSpace = new Vector3(clipSpace.X / clipSpace.W, clipSpace.Y / clipSpace.W, clipSpace.Z / clipSpace.W);

                // Convertim din NDC (Normalized Device Coordinates) în coordonate ecran (screen space)
                int screenX = (int)((ndcSpace.X + 1.0f) * 0.5f * screenWidth);
                int screenY = (int)((1.0f - ndcSpace.Y) * 0.5f * screenHeight);

                // Verificăm dacă poziția mouse-ului este aproape de proiecția unui vertex
                if (Math.Abs(mouseX - screenX) < 5 && Math.Abs(mouseY - screenY) < 5)
                {
                    return true;
                }
            }
            return false;
        }
        private Matrix4 GetModelViewProjectionMatrix(Vector3 cameraPosition)
        {
            Matrix4 model = Matrix4.CreateTranslation(Position);
            Matrix4 view = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 4.0f / 3.0f, 0.1f, 100.0f);
            return model * view * projection;
        }
        private Color GenerateRandomColor()
        {
            Random rand = new Random();
            return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(Position);
            GL.Rotate(Rotation.X, 1, 0, 0);
            GL.Rotate(Rotation.Y, 0, 1, 0);
            GL.Rotate(Rotation.Z, 0, 0, 1);
            GL.Scale(ScaleFactors);

            GL.Color3(Color);
            GL.Begin(PrimitiveType.Quads);

            foreach (var face in Faces)
            {
                foreach (var index in face)
                {
                    GL.Color3(GenerateRandomColor());
                    GL.Vertex3(Vertices[index]);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
