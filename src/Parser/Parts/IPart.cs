using System.Text;

namespace Parser
{
    public interface IPart
    {
        string Print();

        /// <summary>
        /// Render this part to a stringbuilder
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="renderer"></param>
        /// <param name="argIndex">index of the element in <see cref="args"/>. </param>
        /// <param name="args">all args</param>
        /// <returns>new index</returns>
        int RenderPart(StringBuilder sb, Renderer renderer, int argIndex, object[] args);

        /// <summary>
        /// Render this part to a stringbuilder, use the index in the part
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="renderer"></param>
        /// <param name="args">all args</param>
        /// <returns>new index</returns>
        void RenderPartIndexed(StringBuilder sb, Renderer renderer, object[] args);

    }
}