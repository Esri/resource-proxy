<?php

/**
 * Plugin Name: FpOnlineRateTable
 * Plugin URI: http://unknown
 * Description: A client for the OnlineRateTable
 * Version: 00.01.15.444000
 * Author: Carsten Scharfenberg
 * Author URI: http://unknown
 * License: FP internal use only
 */

// Service URL:
// http://localhost/wordpress/wp-content/plugins/FpOnlineRateTable/3rdParty/lib/resource-proxy/proxy.php?http://localhost:33115/api/RateCalculation/GetActiveTables?clientUtcDate=10/30/2015
//
// Hinweis:
// Der resource-proxy verwendet die Enviroment Variables "HTTP_PROXY" und
// "NO_PROXY" um die System Proxy Einstellungen zu übernehmen (das läßt sich
// leider nicht abschalten...)

namespace FP\Web\Portal\FpOnlineRateTable;

// Exit if accessed directly
if (!defined('ABSPATH')) {
    exit;
}

require_once 'src/Utils/GlobalLogger.php';
require_once 'src/Utils/GlobalLoggerConfigXml.php';
require_once 'src/Utils/Wordpress/CustomJavascriptWrapper.php';
require_once 'src/Utils/Wordpress/CustomJavascriptWrapperConfigXml.php';
require_once 'src/Utils/Wordpress/LocalizationTextDomain.php';
require_once 'src/Plugin/Widget.php';
require_once 'src/Plugin/WidgetConfigXml.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLoggerConfigXml;
use FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;
use FP\Web\Portal\FpOnlineRateTable\src\Plugin\WidgetConfigXml;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\CustomJavascriptWrapper;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\CustomJavascriptWrapperConfigXml;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\LocalizationTextDomain;


class CannotLoadConfigFileException extends \RuntimeException {
    public function __construct($configFile, $previous = null) {
        parent::__construct(
                'Cannot load config file "' . $configFile .'"',
                0, $previous);
    }
}


class Plugin {
    
    const CONFIG_FILE = 'config.xml';
    
    static private $instance;
    
    private $config;
    private $textDomain;
    
    
    private function __construct() {
        
        // read config file
        $this->loadConfig();
        
        // use config file to initialize logger settings.
        // Note: if 'loadConfig' fails (the failure is logged using
        // GlobalLogger) GlobalLogger uses it's default configuration.
        $this->initLogger();
        
        $this->initWordpressHelper();
        $this->initLocalization();
        $this->registerWidget();
    }
    
    private function loadConfig() {
        
        $fileName = path_join(__DIR__, self::CONFIG_FILE);
        $xml = file_get_contents($fileName);
        if(false === $xml) {
            throw new CannotLoadConfigFileException($fileName);
        }
        
        $this->config = new \SimpleXMLElement($xml);
    }
    
    private function initLogger() {
        
        $loggerConfig = new GlobalLoggerConfigXml($this->config);
        GlobalLogger::readInitialConfig($loggerConfig);
    }
    
    private function registerWidget() {
        
        $widgetConfig = new WidgetConfigXml($this->config);
        Widget::registerOnAction($widgetConfig, $this->textDomain);
    }
    
    private function initWordpressHelper() {
        
        $customerJavascriptConfig
                = new CustomJavascriptWrapperConfigXml($this->config);
        CustomJavascriptWrapper::readConfiguration($customerJavascriptConfig);
    }
    
    private function initLocalization() {
     
        $languageDir = 'languages';
        $this->textDomain = new LocalizationTextDomain(
                $languageDir, 'FpOnlineRateTable');
     
        // Note:
        // the TextDomain will be globally loaded in the admin backend. In the
        // frontend it won't be loaded before the widget is rendered.
        $this->textDomain->loadOnAction('admin_init');
    }
    
    static public function init() {
        
        try {
            if(!isset(self::$instance)) {
                self::$instance = new self();
            }
        } catch (\Exception $ex) {
            GlobalLogger::addError($ex);
        }
    }
}

Plugin::init();
