using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	[Obsolete("For internal use only. If you want to use iText, please use a dependency on iText 7. ")]
    public class RevReqContent
		: Asn1Encodable
	{
		private readonly Asn1Sequence content;
		
		private RevReqContent(Asn1Sequence seq)
		{
			content = seq;
		}

		public static RevReqContent GetInstance(object obj)
		{
			if (obj is RevReqContent)
				return (RevReqContent)obj;

			if (obj is Asn1Sequence)
				return new RevReqContent((Asn1Sequence)obj);

			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public RevReqContent(params RevDetails[] revDetails)
		{
			this.content = new DerSequence(revDetails);
		}

		public virtual RevDetails[] ToRevDetailsArray()
		{
			RevDetails[] result = new RevDetails[content.Count];
			for (int i = 0; i != result.Length; ++i)
			{
				result[i] = RevDetails.GetInstance(content[i]);
			}
			return result;
		}

		/**
		 * <pre>
		 * RevReqContent ::= SEQUENCE OF RevDetails
		 * </pre>
		 * @return a basic ASN.1 object representation.
		 */
		public override Asn1Object ToAsn1Object()
		{
			return content;
		}
	}
}
