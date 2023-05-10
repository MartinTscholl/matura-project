#!/bin/bash

# Check if the script is being run as root
if [[ $EUID -ne 0 ]]; then
   echo "This script must be run as root"
   exit 1
fi

# Define the functions for installing and uninstalling
install() {
  # Build the cs projects
  dotnet publish -c Release > /dev/null
  # Debugging
  # dotnet publish -c Release
  # Create the /opt/crypass directory
  mkdir -p /opt/crypass
  # Copy the published files into /opt/crypass
  cp -r ./crypass/bin/Release/net6.0/publish/* /opt/crypass/
  # Copy the crypass.sh script into /usr/bin
  cp ./Crypass /usr/bin/
  # Make crypass.sh executable
  chmod +x /usr/bin/Crypass
 
  user_home=$(getent passwd "$SUDO_USER" | cut -d: -f6)
  if [ ! -d "$user_home/.config/crypass" ]; then
    # Create the crypass config directory
    mkdir -p $user_home/.config/crypass
    
    # Create the crypass log directory
    cp -r ./config/* $user_home/.config/crypass
    chown -R $SUDO_USER $user_home/.config/crypass
  fi
  
  echo "Crypass installed successfully!"
}

clean-install() {
  remove
  install
}

uninstall() {
  # Remove the /opt/crypass directory
  rm -rf /opt/crypass
  # Remove the crypass.sh script from /usr/bin
  rm /usr/bin/Crypass
  # Remove the crypass binary directory 
  rm -rf ./crypass/bin
  # Remove the crypass obj directory
  rm -rf ./crypass/obj

  echo "Crypass uninstalled successfully!"
}

remove() {
  uninstall

  user_home=$(getent passwd "$SUDO_USER" | cut -d: -f6)
  rm -rf $user_home/.config/crypass
  
  echo "Crypass removed successfully!"
}

# Parse the command-line arguments
case "$1" in
  install)
    install
    ;;
  uninstall)
    uninstall
    ;;
  clean-install)
    clean-install
    ;;
  remove)
    remove
    ;;
  *)
    echo "Usage: $0 {install|uninstall|clean-install|remove}"
    exit 1
esac

exit 0
