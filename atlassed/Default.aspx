﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Atlassed.Default" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <title>Atlassed</title>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <link rel="stylesheet" href="Scripts/select2-3.4.6/select2.css" />
    <link rel="stylesheet" href="Content/site.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/select2-3.4.6/select2.min.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/fabric.js"></script>
    <script type="text/javascript" src="Scripts/currentContext.js"></script>
    <script type="text/javascript" src="Scripts/canvasInteractions.js"></script>
    <script type="text/javascript" src="Scripts/main.js"></script>
</head>
<body>
    <div id="LoginWrapper">
        <div id="Logo" class="homepage">Atlassed</div>
        <input id="username" class="box-shadow" placeholder="Username" type="text"/>
        <br/>
        <input id="password" class="box-shadow" placeholder="Password" type="password"/>
        <br/>
        <button id="login">Login</button>
    </div>
    <div id="AppWrapper">
        <!--Search Area-->
        <div id="Search" class="floating-container homepage" tabindex="1">
            <div id="Logo" class="homepage">Atlassed</div>
            <div id="Instructions" class="homepage-only">Search for people, rooms and buildings...</div>
            <div id="SearchBox">
                <input autofocus class="search-icon" />
                <!--<button id="SearchButton" onclick="return false;"></button>-->
            </div>
            <div id="SearchResultSummary" style="display: none;"><span></span><a id="ClearSearch" style="margin-left: 5px">Clear Search</a></div>
            <div id="SearchResults">
                <div id="DetailsCard" style="display: none;">
                    <a id="BackToResults">&larr; Back to results</a>
                    <div id="DetailsCardTitle"></div>
                    <table id="DetailsCardTable">
                    </table>
                    <div class="admin-only data"><a class="remove">Remove</a> <a class="edit">Edit</a></div>
                </div>
                <div id="SearchResultList"></div>
                <div id="SearchResultTemplate" class="search-result data" style="display: none;">
                    <div class="left">
                        <div class="primary-text"></div>
                        <div class="secondary-text"></div>
                        <div id="AdminActions" class="admin-only"><a class="remove">Remove</a> <a class="edit">Edit</a></div>
                    </div>
                    <div class="right"></div>
                </div>
            </div>
        </div>
    
        <!--Toolbar-->
        <div id="HideAdminToolbar" runat="server">
            <div id="Toolbar" class="floating-container">
                <div id="MouseModes" class="toolbar-button-group">
                    <button id="T_SELECT" class="toolbar-button toggle-button selected">Select</button>
                    <button id="T_ERASE" class="toolbar-button toggle-button">Erase</button>
                    <!--<button id="T_DRAW_RECT" class="toolbar-button toggle-button">Define Room</button>-->
                    <!--<button id="T_DRAW_POLY" class="toolbar-button toggle-button">Draw Polygon</button>-->
                    <button id="T_TAG_WORKSTATION" class="toolbar-button toggle-button">Tag Location</button>
                </div>
                <div id="DataActions" class="toolbar-button-group">
                    <button id="T_BUILDINGS" class="toolbar-button">Add Building</button>
                    <button id="T_PEOPLE" class="toolbar-button">Add Person</button>
                </div>
            </div>
            <!--Tiles-->
            <div id="UpperRightTiles" class="floating-container">
                <select id="Buildings" class="tile-select"></select>
                <select id="Floors" class="tile-select disabled" disabled>
                    <option>Floor...</option>
                </select>
            </div>
            <div id="LowerLeftTiles" class="floating-container">
                <div id="MapLink" class="tile-link-medium">
                    <div>Map</div>
                </div>
            </div>
            <div id="ZoomTiles" class="floating-container">
                <div>
                    <button id="ZoomIn">+</button>
                </div>
                <div>
                    <button id="ZoomOut">&minus;</button>
                </div>
            </div>
            <div id="CenterTiles" class="floating-container"></div>

            <!--Canvas-->
            <div id="canvasWrapper" style="display: none;">
                <canvas id="mainCanvas">Your browser does not support this.</canvas>
            </div>
            <!--<div id="Watermark"></div>-->
            
            <img id="PinImage" src="Images/pin.png" style="display: none;" alt=""/>

            <!--Dialogs-->
            <div style="display: none;">
                <div id="PersonDialog">
                    <div id="PersonInputs">
                        <label>Name:</label>
                        <input class="person-name" />
                        <br />
                        <label>Extension:</label>
                        <input class="person-extension" size="6" />
                    </div>
                </div>
                <div id="WorkstationDialog">
                    <label>Number:</label>
                    <input id="WorkstationNumber" />
                    <br />
                    <label>Port:</label>
                    <input id="WorkstationPort" />
                    <br />
                    <label>Assigned To:</label>
                    <select id="AssignWorkstation"></select>
                </div>
                <div id="SpaceDialog">
                    <label>Number:</label>
                    <input id="SpaceNumber" />
                    <br />
                    <label>Name:</label>
                    <input id="SpaceName" />
                </div>
                <div id="BuildingDialog">
                    <div id="BuildingInputs">
                        <label>Name:</label>
                        <input class="building-name" />
                        <br />
                        <label>Address:</label>
                        <input class="building-address" />
                        <br />
                        <br />
                        <div id="FloorList" style="border: 1px solid black;">
                            <table>
                                <thead>
                                    <tr>
                                        <th>Floor Number</th>
                                        <th>File</th>
                                        <th></th>
                                    </tr>
                                    <tr class="template" data-floor-id="0">
                                        <td>
                                            <input class="floor-number" size="2" maxlength="3" /></td>
                                        <td>
                                            <input class="floor-filename" size="20" /></td>
                                        <td><a class="floor-remove">X</a></td>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                            <a class="add-floor">Add Floor</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!--Google Map-->
    <div id="GoogleMapContainer" style="display: none;">
        <iframe id="GoogleMapFrame"></iframe>
    </div>
</body>
</html>