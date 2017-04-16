OpenCoverConverter
=========

OpenCoverConverter - Converts opencover format into sonarqube format, keeping branch/coverage

## License
This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

## Usage

    "Usage: OpenCoverPathConverter [OPTIONS]"
    "Converts opencover xml to sonar generic coverage report"
    
    "Options:"
    "    /D|/d:<directory to look for files>"
    "    /P|/p:<Pattern of files to search>"
    "    /E|/e:<End Path to Use for path replacement in Report>"
    "    /S|/s:<Search string for path replace in report>"
    "    /R|/r:<covert reports : search string>"
    "    /O|/o:<out file>"
    "    /X|/x:<output xml>"

### /e /s arguments
The /e and /s arguments are used when you need to transform the paths into the final analysis folder. For example in teamcity its common, that checkout folders to be named like c:\buildagent\<hash>. This hash normally its not guaranted to be the same when you move reports between agents. With this 2 arguments you can transform those path to the paths the sonar analysis actually uses.

## Output formats
It generated generic coverage format that you can feed then in sonar using sonar.coverageReportPaths see https://docs.sonarqube.org/display/SONAR/Generic+Test+Data 
It generates json coverage format for https://github.com/jmecsoftware/sonarqube-testdata-plugin 

The argument /x will perform conversion to generic test data, by default the converter generates json coverage that you can use with the testdata-plugin

## Why the json coverage
You must be asking why another coverage format? The generic test data is indeed a good approach, and this format would not be needed if the generic test data would support merging condition coverage. 
The xml format provides only conditions and covered conditions. So if you have 2 reports with different data the behaviour becomes undefined. To support this, the branch/conditions must be present in the report. the test-data-plugin does exactly that. 

## Examples

1. generate generic coverage report xml
   OpenCoverPathConverter /d c:\reports-folder /p *.xml /e Folder /r d:\usersrc /x /o merged.cov.xml
   
2. generate generic coverage report json
   OpenCoverPathConverter /d c:\reports-folder /p *.xml /e Folder /r d:\usersrc /o merged.cov.json
