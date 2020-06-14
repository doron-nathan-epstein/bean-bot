const Promise = require("bluebird");
const { ClientCredentials } = require("simple-oauth2");

module.exports = blizzardAuth;

function blizzardAuth() {
  const credentials = {
    client: {
      id: process.env.BLIZZARD_CLIENTID,
      secret: process.env.BLIZZARD_CLIENTSECRET,
    },
    auth: {
      tokenHost: "https://us.battle.net",
    },
  };

  let token = null;

  this.getToken = function () {
    if (token === null || token.expired()) {
      const client = new ClientCredentials(credentials);
      return client.getToken().then((t) => {
        this.token = t;
        return t.token.access_token;
      });
    } else {
      return Promise.resolve(this.token.token.access_token);
    }
  };
}
