# BirdsiteLIVE

[![ASP.NET Core Build & Tests](https://github.com/jim60105/BirdsiteLive/actions/workflows/dotnet-core.yml/badge.svg?branch=master)](https://github.com/jim60105/BirdsiteLive/actions/workflows/dotnet-core.yml)
[![docker_publish](https://github.com/jim60105/BirdsiteLive/actions/workflows/docker_publish.yml/badge.svg?branch=master)](https://github.com/jim60105/BirdsiteLive/actions/workflows/docker_publish.yml)

This project is another *fork* of [the fork of BirdsiteLIVE from pasture](https://git.gamers.exposed/pasture/BirdsiteLIVE). Changes made in this fork by jim60105(me) are:

* Merge the latest changes from pasture/BirdsiteLIVE into NicolasConstant/BirdsiteLive
* Migration to .NET 7
* Update nuget packages
* Use Workstation GC for my docker usage <https://github.com/jim60105/BirdsiteLive/commit/ebd0c07ef1334538095625631e3ea7c12869f69e>

This fork is also available as a Docker image as `jim60105/birdsitelive` and `ghcr.io/jim60105/birdsitelive`.

The pasture/BirdsiteLIVE project's original README is as follows:

This project is a *fork* of [the original BirdsiteLIVE from NicolasConstant](https://github.com/NicolasConstant/BirdsiteLive). This fork runs in production on [a large BirdsiteLIVE instance](https://twtr.plus). Changes made in this fork include:

* Rework About page entirely - also disclose unlisted accounts and federation restrictions
* Cache Tweets so that, for example, Announces do not hit rate limits
* Allow replacing and redirecting to twitter.com in Tweets to other domains (i.e. Nitter instances)
* Verified checkmarks on [verified](https://twitter.com/verified) Twitter users
* Proper remote follow form on user pages
* Mark individual Tweets as potentially sensitive
* Define and enforce a maximum follow count limit
* Define and enforce a maximum Tweet fetch age using snowflakes
* (Optional) send quote-RTs as Soapbox-style quote posts

This fork is also available as a Docker image as `pasture/birdsitelive`.

The project's original README is as follows:

![Test](https://github.com/NicolasConstant/BirdsiteLive/workflows/.NET%20Core/badge.svg?branch=master&event=push)

# BirdsiteLIVE

## About

BirdsiteLIVE is an ActivityPub bridge from Twitter, it's mostly a pet project/playground for me to handle ActivityPub concepts. Feel free to deploy your own instance (especially if you plan to follow a lot of users) since it use a proper Twitter API key and therefore will have limited calls ceiling (it won't scale, and it's by design).

## State of development

The code is pretty messy and far from a good state, since it's a playground for me the aim was to understand some AP concepts, not to provide a good state-of-the-art codebase. But I might refactor it to make it cleaner. 

## Official instance 

There's none! Please read [here why I've stopped it](https://write.as/nicolas-constant/closing-the-official-bsl-instance).

## Installation

I'm providing a [docker build](https://hub.docker.com/r/nicolasconstant/birdsitelive) (linux/amd64 only). To install it on your own server, please follow [those instructions](https://github.com/NicolasConstant/BirdsiteLive/blob/master/INSTALLATION.md). More [options](https://github.com/NicolasConstant/BirdsiteLive/blob/master/VARIABLES.md) are also available.

Also a [CLI](https://github.com/NicolasConstant/BirdsiteLive/blob/master/BSLManager.md) is available for adminitrative tasks.

## License

This project is licensed under the AGPLv3 License - see [LICENSE](https://github.com/NicolasConstant/BirdsiteLive/blob/master/LICENSE) for details.

## Contact

You can contact me via ActivityPub <a rel="me" href="https://fosstodon.org/@BirdsiteLIVE">here</a>.
