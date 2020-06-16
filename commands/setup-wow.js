const Promise = require("bluebird");
const fetch = require("node-fetch");

fetch.Promise = Promise;

module.exports = {
  name: "setup-wow",
  description:
    "Setup the server to use a default World of Warcraft guild for WoW queries.",
  guildOnly: true,
  adminOnly: true,
  args: true,
  usage: "[region] [realm] [guild-name]",
  async execute(app, message, args) {
    if (args.length !== 3) {
      return message.reply("Incorrect amount of arguments recieved.");
    }

    const regionName = args[0];
    const region = app.dao.blizzard.getRegion(regionName);
    if (region === undefined) {
      return message.reply(`The region ${regionName} is not valid.`);
    }

    const realmSlug = args[1];
    const realm = app.dao.blizzard.getRealm(region.id, realmSlug);
    if (realm === undefined) {
      return message.reply(
        `The realm ${realmSlug} (${regionName}) is not valid.`
      );
    }

    const guildName = args[2];
    app.authProviders.blizzard
      .getToken()
      .then((token) => {
        return verifyGuild(token, region, realm, guildName);
      })
      .then((isValidGuild) => {
        if (!isValidGuild) {
          return message.reply(
            `The guild ${guildName} is not valid for realm ${realmSlug} (${regionName}).`
          );
        }

        app.dao.guildWow.replace(
          message.guild.id,
          region.name,
          realm.slug,
          guildName
        );
        const response = `Succesfully set the default WoW Guild for ${message.guild.name} to ${guildName}@${realmSlug} (${regionName}).`;
        console.log(response);
        message.reply(response);
      });
  },
};

function verifyGuild(token, region, realm, guild) {
  const url = `${region.host}/data/wow/guild/${realm.slug}/${guild}?namespace=profile-${region.region}&locale=${region.locale}&access_token=${token}`;
  return fetch(url)
    .then((res) => res.json())
    .then((res) => {
      return res.code ? false : true;
    });
}
