class DeviceAuthenticator {
    constructor() {
        this.isSupported = window.PublicKeyCredential &&
            PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable &&
            window.navigator.credentials;
    }

    async isBiometricAvailable() {
        if (!this.isSupported) return false;

        try {
            const available = await PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable();
            return available;
        } catch (e) {
            console.error('Error checking biometric availability:', e);
            return false;
        }
    }

    async authenticate() {
        const biometricAvailable = await this.isBiometricAvailable();

        if (biometricAvailable) {
            return await this.authenticateWithBiometric();
        } else {
            return await this.authenticateWithPIN();
        }
    }

    async authenticateWithBiometric() {
        try {
            const publicKey = {
                challenge: new Uint8Array(32),
                rp: {
                    name: "GeoAttendance System"
                },
                user: {
                    id: new Uint8Array(16),
                    name: "employee",
                    displayName: "Employee"
                },
                pubKeyCredParams: [{
                    type: "public-key",
                    alg: -7
                }],
                authenticatorSelection: {
                    authenticatorAttachment: "platform",
                    userVerification: "required"
                },
                timeout: 60000
            };

            await navigator.credentials.create({ publicKey });
            return true;
        } catch (e) {
            console.error('Biometric authentication failed:', e);
            return false;
        }
    }

    async authenticateWithPIN() {
        return new Promise((resolve) => {
            const pin = prompt("Please enter your PIN to mark attendance:");
            // In a real implementation, you would validate the PIN against a stored hash
            // For demo purposes, we're using a simple check
            resolve(pin !== null && pin.length >= 4);
        });
    }
}