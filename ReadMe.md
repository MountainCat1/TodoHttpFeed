# Todo Http Feed

## Description
Project consists of two services:
1. TodoApp _aka_ Feed Server - A service that provides a REST API to manage a list of todo items. And an endpoint `api/todo-items/feed` that provides a feed of todo items according to https://www.http-feeds.org/.
2. CmdApp _aka_ Feed Client - A command line application that consumes the feed from the Feed Service and saves the todo items to a json file.
## How To Run
1. Clone the repository
```bash
git clone https://github.com/MountainCat1/TodoHttpFeed.git
```
2. Run
```bash
docker compose up
```
## Notes:
- The Feed Server uses local mysql database to store todo items. Which means recreating the container will result in losing the data.
- The Feed Client saves the todo items to a file named `todo-items.json`.