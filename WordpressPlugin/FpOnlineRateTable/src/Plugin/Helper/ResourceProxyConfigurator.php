<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Helper;


class FileAccessError extends \RuntimeException {};
class XmlStructureError extends \RuntimeException {};

/*******************************************************************************
 * 
 * Note:
 * this file is currently not in use as it is bad practice to write into the
 * plugin directory. Nevertheless for the moment beeing this file is preserved.
 * A solution could be to use the Wordpress WP_Filesystem class to access the
 * files. The better solution would be to modify the resoure proxy to get rid
 * of the need to modify the proxy file at all.
 * 
 ******************************************************************************/


/**
 * Description of ResourceProxyConfigurator
 *
 * @author scharfenberg
 */
abstract class ResourceProxyConfigurator {
    
    const configFile = "/vendor/Yall1963/resource-proxy/PHP/proxy.config";
    
    static public function setLogFile($logFile) {
        
        $xml = self::readConfig();
        
        $xml->attributes()->logFile = $logFile;
        
        self::writeConfig($xml);
    }
    
    static public function setDestinationServiceUrl($url) {
        
        $xml = self::readConfig();
        
        // Note: we always change the first serverUrl entry in the file
        $serverUrls = $xml->serverUrls;
        if(!$serverUrls) {
            throw new XmlStructureError(
                    "Expected the resource proxy xml file to contain the tag 'serverUrls'");
        }
        
        $serverUrl = $serverUrls->serverUrl;
        if(!$serverUrl) {
            throw new XmlStructureError(
                    "Expected the resource proxy xml file to contain at least one 'serverUrl' tag");
        }
        if(is_array($serverUrl)) {
            $serverUrl = $serverUrl[0];
        }
        
        $serverUrl->attributes()->url = $url;
        
        self::writeConfig($xml);
    }
    
    static private function configPath() {
        
        $pluginPath = dirname(dirname(dirname(plugin_dir_path(__FILE__))));
        $configPath = $pluginPath . self::configFile;
        
        return $configPath;
    }
    
    static private function readConfig() {
        
        $fileName = self::configPath();
        $xml = simplexml_load_file($fileName);
        if(false === $xml) {
            throw new FileAccessError(
                    "Error when trying to load resource-proxy config file '{$fileName}'");
        }
        
        return $xml;
    }
    
    static private function writeConfig(\SimpleXMLElement $xml) {
        
        $fileName = self::configPath();
        $result = $xml->saveXML($fileName);
        
        if(false === $result) {
            throw new FileAccessError(
                    "Error when trying to save resource-proxy config file '{$fileName}'");
        }
    }
}
