const Promise = require("bluebird");

module.exports = guildWow;

function guildWow(connection) {
  this.db = connection;

  this.setup = function () {
    return new Promise((resolve) => {
      const sql = this.db.prepare(
        "CREATE TABLE IF NOT EXISTS guildWow ('guildID' VARCHAR PRIMARY KEY, 'region' VARCHAR, 'realm' VARCHAR, 'guild' VARCHAR)"
      );
      sql.run();
      resolve(console.log("\tGuild-Wow: ✓"));
    });
  };

  this.action = function (guildId, region, realm, guildName) {
    return new Promis((resolve) => {
      const sql = this.db.prepare(
        "INSERT INTO guildWow (guildId, region, realm, guild) VALUES (?, ?, ?, ?)"
      );
      resolve(sql.run(guildId, region, realm, guildName));
    });
  };
}
