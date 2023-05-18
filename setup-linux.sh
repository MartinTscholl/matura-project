#!/bin/bash

# Color codes
RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
PURPLE='\033[0;35m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Icons
# ✔ ✘ ℹ ⚠ ⚡ ⚙ ❓ ❗

# Check if the script is being run as root
if [[ $EUID -ne 0 ]]; then
    echo "This script must be run as root"
    exit 1
fi

display_usage() {
    echo -e "${BLUE}ℹ${NC} Usage: $0 [command] [option]"
    echo -e "${BLUE}ℹ${NC} Commands:"
    echo -e "${BLUE}ℹ${NC}   install        Install crypass"
    echo -e "${BLUE}ℹ${NC}     -v, --verbose  Display the output of the build process"
    echo -e "${BLUE}ℹ${NC}   uninstall      Uninstall crypass"
    echo -e "${BLUE}ℹ${NC}   clean-install  Remove and re-install crypass"
    echo -e "${BLUE}ℹ${NC}   remove         Remove crypass and its configurations files"
}

install() {
    # Set the script to exit if any command fails
    set -e
    
    cd "$(dirname $0)"
    
    if [ ! -d "/etc/crypass" ]; then
        # Create the crypass config directory
        mkdir -p /etc/crypass
        # Create the crypass log directory
        cp -r ./config-linux/* /etc/crypass
        # Make the crypass config directory accessible by everyone
        chmod -R a+r /etc/crypass
    fi
    
    if [ ! -f "/var/log/crypass.log" ]; then
        # Create the crypass log file
        touch /var/log/crypass.log
        # Make the crypass log file writable by everyone
        chmod a+w /var/log/crypass.log
    fi
    
    if [[ $1 == "--verbose" || $1 == "-v" ]]; then
        # Build the cs projects
        dotnet publish -c Release
    else
        # Build the cs projects
        dotnet publish -c Release > /dev/null
    fi
    
    # Create the /opt/crypass directory
    mkdir -p /opt/crypass
    # Copy the published files into /opt/crypass
    cp -r ./crypass/bin/Release/net6.0/publish/* /opt/crypass/
    # Make the compiled files readable and executable by everyone
    chmod -R a+rX,a-w /opt/crypass
    
    # Create the crypass executable into /usr/bin
    touch /usr/local/bin/crypass
    echo "# This file was automatically created by crypass on installation. Do not edit!" > /usr/local/bin/crypass
    echo "#!/bin/bash" >> /usr/local/bin/crypass
    echo "/opt/crypass/MaturaProject \$@" >> /usr/local/bin/crypass
    # Make the crypass executable by everyone
    chmod a+x,a-w /usr/local/bin/crypass
    
    echo -e "${GREEN}✔${NC} Crypass ${GREEN}installed${NC} successfully!"
    
    # Unset the script to exit if any command fails
    set +e
}

clean-install() {
    remove
    install
}

uninstall() {
    cd "$(dirname $0)"
    
    # Remove the crypass binary directory
    rm -r ./crypass/bin
    # Remove the crypass obj directory
    rm -r ./crypass/obj
    # Remove the /opt/crypass directory
    rm -r /opt/crypass
    # Remove the crypass.sh script from /usr/bin
    rm /usr/local/bin/crypass
    
    echo -e "${GREEN}✔${NC} Crypass ${GREEN}uninstalled${NC} successfully!"
}

remove() {
    uninstall
    
    # Remove the crypass config directory
    rm -r /etc/crypass
    # Remove the crypass log files
    rm /var/log/crypass.log
    
    echo -e "${GREEN}✔${NC} Crypass ${GREEN}removed${NC} successfully!"
}

# Parse the command-line arguments
case "$1" in
    install)
        install $2
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
        display_usage
        exit 1
esac

exit 0
