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

  this.replace = function (guildId, region, realm, guildName) {
    const sql = this.db.prepare(
      "REPLACE INTO guildWow (guildId, region, realm, guild) VALUES (?, LOWER(?), LOWER(?), LOWER(?))"
    );
    sql.run(guildId, region, realm, guildName);
  };
}
