<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

require_once 'IWidgetConfig.php';


/**
 * Description of WidgetConfigXml
 *
 * @author scharfenberg
 */
class WidgetConfigXml implements IWidgetConfig {

    private $id;
    private $htmlClass;
    private $shortcode;
    private $serviceProxyPath;
    
    
    public function __construct(\SimpleXMLElement $config) {
        
        $this->id = (string)$config->FpOnlineRateCalculatorWidget->Id;
        $this->htmlClass
                = (string)$config->FpOnlineRateCalculatorWidget->HtmlClass;
        $this->shortcode
                = (string)$config->FpOnlineRateCalculatorWidget->Shortcode;
        $this->serviceProxyPath
                = (string)$config->FpOnlineRateCalculatorWidget->ServiceProxyPath;
    }
    
    /**
     * This is the Wordpress internal Id of the widget
     */
    public function id() {
        return $this->id;
    }
    
    /**
     * This is the html class name used for the widget container
     */
    public function htmlClass() {
        return $this->htmlClass;
    }
    
    /**
     * This is the shortcode used to place the widget on a page
     */
    public function shortcode() {
        return $this->shortcode;
    }
    
    /**
     * The path relative to the plugin directory of the OnlineRateTable service
     * proxy.
     * Note: Currently the resource-proxy is in use that requires the
     * original service URL to be passed as parameter. This URL is meant to be
     * specified without this parameter.
     */
    public function serviceProxyPath() {
        return $this->serviceProxyPath;
    }
}
