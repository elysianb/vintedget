# Installation

- Install Dotnet Framework 4.8

https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer

- Extract binaries from a release into a local directory. Then, use it

`vget --help` 

# Internals

[How Vinted API is used](doc/How-to-Vinted-API.md)

# Documentation

NAME

        vget - get full size photos from Vinted web sites

SYNOPSIS

        vget [options / URL]

DESCRIPTION

        vget is a tool for getting full size photos of items published to 
        Vinted websites. And store them locally with metadata like size, 
        brand, itemId.

        It also provide features for extracting photos of a part of your 
        favorite list of items from Vinted. And also photos extraction from 
        your conversation thread.

        It also provide features to automate large batch photos extraction and
        storage in an organized and configurable folder structure

URL

        The URL of the item you want to download full size photos. The photos 
        are stored with a name like this:

          <itemId>-<brand>-T<size>-<photoId>-<photo counter>-<filename>

        If available, the profile photo of the seller is also downloaded.

        Example:
          vget https://www.vinted.fr/items/1234567890-category-example-of-item

OPTIONS

        Options start with one or two dashes. Many of the options require an 
        additional value next to them.

        Options requires a space between it and its value.

        --login
                Open a persistent session. Session state is stored in vget 
                local storage.

                Additional needed arguments are the following :
                -T, --token
                    JWT token for read operations related to user. For example,
                    token corresponds to the cookie nammed access_token_web from 
                    host .www.vinted.fr

                    for practical usage, store token in a shell variable and
                    pass this variable as parameter

                Example:
                  vget --login -T $token

        --whoami
                If logged, display the login stored in vget local storage

        --logout
                Remove a persistent session information from vget local storage

        -u, --url
                See also URL

                Example:
                  vget --url https://www.vinted.fr/items/1234567890-category-example-of-item

        -nb, --new-batch
                Create a sample batch file. A batch file is a text file that
                lists Vinted items urls, grouped by categories. Categories will be
                used to download photos in a specific folder.

                The sample batch file generated contains examples of categories 
                and Vinted items urls.

                In the following file example, a folder structure will be 
                created if not existe, and photos will be download into it. 
                For xbox games, to folder ./console/xbox, and for ps5 game, 
                to folder ./console/ps5

                # --- Example file ---
                # example of a comment line

                # example of items in current folder
                https://www.vinted.fr/items/5216666706-jeux-mario-lapins-cretins-switch
                https://www.vinted.fr/items/5218415510-super-mario-3d-all-stars

                # example of a sub-folder with 2 items
                :console/xbox
                https://www.vinted.fr/divertissement/xbox-one-2381/1234567890-jeux1
                https://www.vinted.fr/divertissement/xbox-one-2381/1234567891-jeux2

                # example of a sub-folder with 2 items
                :console/ps5
                https://www.vinted.fr/divertissement/playstation-5-2390/1234567892-jeux1
                https://www.vinted.fr/divertissement/playstation-5-2390/1234567893-jeux2
                # --- End of example file ---

                Examples:
                  vget --new-batch category.txt
                  vget --new-batch
                  vget -nb category.txt
                  vget -nb

                When no filename is specified, default name is vget.batch.txt

                if vget local storage contains a file named BatchTemplate.txt,
                this file is copyed to working directory.

        -b, --batch         
                Run the download of all full size photos of all Vinted items
                listed in the batch file. Photos are stored to the folders 
                specified in batch file.

                Examples:
                  vget --batch
                  vget --batch category.txt
                  vget -b
                  vget -b category.txt

                When no filename is specified, default name is vget.batch.txt

                Additional parameters

                -so, --statistics-only
                  Only display batch file statistics, without downloading 
                  photos.

                  Examples:
                    vget --batch category.txt --statistics-only
                    vget -b category.txt -so

        -cls, --clean
                Remove log files recursively from working directory. Theses 
                files are the following:
                  *.vget-summary.log
                  *.vget-response.json
                  *.vget-response.html

        -t, --thread
                Download photos from a user thread (inmail messages between 
                Vinted users). Photos are saved in a folder named with the 
                thread id, and a .vget-response.html file and also a 
                .vget-response.json file are created in the folder. 

                If you have previously executed vget --login command, you can
                use it like the following examples.

                Examples:
                  vget --thread 123456
                  vget --t 123456
                
                Additional arguments are needed if you want to download photos 
                without login or if you want to use a particular identity :
                -T, --token

                Examples:
                  vget --thread 123456 -T $token
                  vget --t 123456 -T $token

        -f, --favorites
                Execute operation on favorites list of authenticated user. 
                Favorites are saved in a csv file with the user id

                Available operations:
                -l, --list
                  List favorites. Value can be list-csv for csv file,
                  otherwise a txt batch file will be generated.
                  
                  Examples:
                    vget --favorites --list
                    vget --favorites --list list-csv

                -dl, --download
                  Download full size photos from items in user's favorites

                  Examples:
                    vget --favorites --download
                    vget --favorites -dl
                    vget -f -dl

                Additional operation parameters:
                -il, --itemlimit
                  If item url found, limit operation after the item limit.

                  Examples:
                    vget --favorites -l -il https://www.vinted.fr/items/1234567890-sample
                    vget --favorites -dl -il https://www.vinted.fr/items/1234567890-sample

                Additional arguments are needed if you want to download photos 
                without login or if you want to use a particular identity :
                -T, --token

                Examples:
                  vget --favorites -l -T $token
                  vget --favorites -dl -T $token

        -h, --help
                Display this manual page

        -v, --version
                Display the current version number

ADDITIONAL OPTIONS

        -d, --delay       
                Delay between two downloads from Vinted web site, in 
                milliseconds. Default value is 500ms.

        -r, --max-retry
                Maximum retry when network issue occurs. default value is 10

        -o, --output      
                Output folder where to save downloaded images. Default value 
                is current directory (.)

LOCAL STORAGE

        vget store settings and state in a local folder .vget ; .vget folder 
        can be located in user home directory, or, in current folder. If a 
        .vget folder exists in current folder, all settings are taken from it.
