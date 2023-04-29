# Setup Docker base on the link: https://docs.docker.com/desktop/install/windows-install/

# Setup MongoDB on Docker
cd Setup
docker build -t mongodb .
docker run --detach --name=mongodb --publish 27017:27017 mongodb

# Connect to MongoDB on Docker
docker exec -it mongodb bash

# Run MongoDB commands
mongosh

# Show database MongoDB
show databases

# Exit MongoDB 
exit

# Run data migration
