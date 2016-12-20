using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Owin.JwtAuth
{
    /// <summary>
    /// Removes a set of authentication types from challenges in <c>WWW-Authenticate</c> headers.
    /// </summary>
    public class AuthChallengeFilterMiddleware : OwinMiddleware
    {
        private readonly string[] _typesToRemove;

        /// <summary>
        /// Instantiates the authentication challenge filter middleware.
        /// </summary>
        /// <param name="next">The next middleware component in the pipeline</param>
        /// <param name="typesToRemove">A list of authentication types to remove from challenges in <c>WWW-Authenticate</c> headers.</param>
        public AuthChallengeFilterMiddleware(OwinMiddleware next, params string[] typesToRemove) : base(next)
        {
            _typesToRemove = typesToRemove;
        }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            context.Response.OnSendingHeaders(x =>
            {
                var headers = (IHeaderDictionary)x;
                var challenges = headers.GetCommaSeparatedValues("WWW-Authenticate");
                if (challenges != null)
                    headers.SetCommaSeparatedValues("WWW-Authenticate", challenges.Except(_typesToRemove).ToArray());
            }, context.Response.Headers);
        }
    }
}