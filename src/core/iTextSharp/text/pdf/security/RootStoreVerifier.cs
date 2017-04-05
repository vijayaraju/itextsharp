/*
 * $Id: RootStoreVerifier.java 5465 2012-10-07 12:37:23Z blowagie $
 *
 * This file is part of the iText (R) project.
 * Copyright (c) 1998-2016 iText Group NV
 * Authors: Bruno Lowagie, Paulo Soares, et al.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License version 3
 * as published by the Free Software Foundation with the addition of the
 * following permission added to Section 15 as permitted in Section 7(a):
 * FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
 * ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
 * OF THIRD PARTY RIGHTS
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Affero General Public License for more details.
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see http://www.gnu.org/licenses or write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA, 02110-1301 USA, or download the license from the following URL:
 * http://itextpdf.com/terms-of-use/
 *
 * The interactive user interfaces in modified source and object code versions
 * of this program must display Appropriate Legal Notices, as required under
 * Section 5 of the GNU Affero General Public License.
 *
 * In accordance with Section 7(b) of the GNU Affero General Public License,
 * a covered work must retain the producer line in every PDF that is created
 * or manipulated using iText.
 *
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the iText software without
 * disclosing the source code of your own applications.
 * These activities include: offering paid services to customers as an ASP,
 * serving PDFs on the fly in a web application, shipping iText with a closed
 * source product.
 *
 * For more information, please contact iText Software Corp. at this
 * address: sales@itextpdf.com
 */

using System;
using System.Collections.Generic;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iTextSharp.text.log;

/**
 * Verifies a certificate against a <code>KeyStore</code>
 * containing trusted anchors.
 */
namespace iTextSharp.text.pdf.security {
	[Obsolete("For internal use only. If you want to use iText, please use a dependency on iText 7. ")]
    public class RootStoreVerifier : CertificateVerifier {
        /** The Logger instance */
	    private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(RootStoreVerifier));

	    /** A key store against which certificates can be verified. */
	    protected List<X509Certificate> certificates  = null;

	    /**
	     * Creates a RootStoreVerifier in a chain of verifiers.
	     * 
	     * @param verifier
	     *            the next verifier in the chain
	     */
	    public RootStoreVerifier(CertificateVerifier verifier) : base(verifier) {}

	    /**
	     * Sets the Key Store against which a certificate can be checked.
	     * 
	     * @param keyStore
	     *            a root store
	     */
        virtual public List<X509Certificate> Certificates {
            set { certificates = value; }
        }

	    /**
	     * Verifies a single certificate against a key store (if present).
	     * 
	     * @param signCert
	     *            the certificate to verify
	     * @param issuerCert
	     *            the issuer certificate
	     * @param signDate
	     *            the date the certificate needs to be valid
	     * @return a list of <code>VerificationOK</code> objects.
	     * The list will be empty if the certificate couldn't be verified.
	     */
	    override public List<VerificationOK> Verify(X509Certificate signCert, X509Certificate issuerCert, DateTime signDate) {
		    LOGGER.Info("Root store verification: " + signCert.SubjectDN);
		    // verify using the CertificateVerifier if root store is missing
		    if (certificates == null)
			    return base.Verify(signCert, issuerCert, signDate);
		    try {
			    List<VerificationOK> result = new List<VerificationOK>();
			    // loop over the trusted anchors in the root store
                foreach (X509Certificate anchor in certificates) {
				    try {
					    signCert.Verify(anchor.GetPublicKey());
					    LOGGER.Info("Certificate verified against root store");
					    result.Add(new VerificationOK(signCert, this, "Certificate verified against root store."));
					    result.AddRange(base.Verify(signCert, issuerCert, signDate));
					    return result;
				    } catch (GeneralSecurityException) {}
			    }
			    result.AddRange(base.Verify(signCert, issuerCert, signDate));
			    return result;
		    } catch (GeneralSecurityException) {
			    return base.Verify(signCert, issuerCert, signDate);
		    }
	    }
	}
}
