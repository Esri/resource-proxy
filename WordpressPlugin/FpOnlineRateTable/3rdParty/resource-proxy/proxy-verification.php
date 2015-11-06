<html>
<head><title>Prerequisites Checker</title></head>
<style>
body{
    margin-left:12px;
    margin-left:30px;
    margin-right:30px;
    margin-bottom:10px;
}
.requirements-div {
    margin-top:16px;
    margin-left:20px;
    margin-right:20px;
    margin-bottom:10px;
}
table {
    border: 1px solid #696969;
    width: 50%;
}
.pass {
    color:#3CB371;
}
.fail {
    color:#FF4500;
}
.warning {
    color:#FFA500;
}
</style>
<body>
<?php

error_reporting(0);

class Paths
{
    public $pearPath;
    public $requestsPath;
}

function include_exists($fileName){
    if (realpath($fileName) == $fileName) {
        return is_file($fileName);
    }
    if ( is_file($fileName) ){
        return true;
    }

    $paths = explode(PATH_SEPARATOR, get_include_path());

    foreach ($paths as $key => $path) {

        $rp = $path . DIRECTORY_SEPARATOR . $fileName;

        if (is_file($rp)) {

            if($fileName == 'PEAR' . DIRECTORY_SEPARATOR .'Registry.php'){

                $path = $rp;
            }

            if($fileName == 'HTTP' . DIRECTORY_SEPARATOR . 'Request2.php'){

                $path = $rp;
            }

            return array('result' => true, 'path' => $path);
        }
    }

    return false;
}

function pear_prerequisutes()
{
    $exists = include_exists('PEAR' . DIRECTORY_SEPARATOR .'Registry.php');
    if($exists['result'])
    {
        require_once 'PEAR/Registry.php';
        $reg = new PEAR_Registry();
        $packages = $reg->listPackages();
        if(in_array('http_request2', $packages))
        {
            $results = array('pear'=>"Pass", 'http_request2'=>"Pass", 'path'=>$exists['path']);
        }else{
            $results = array('pear'=>"Pass", 'http_request2'=>"Fail", 'path'=>$exists['path']);
        }
    }else{
        $results = array('pear'=>"Fail", 'http_request2'=>"Fail", 'path'=>$exists['path']);
    }
    return $results;
}

function http_requests2_exists()
{
    $exists = include_exists('HTTP' . DIRECTORY_SEPARATOR . 'Request2.php');

    if($exists['result'])
    {
        return array('result' => "Pass", 'path'=> $exists['path']);

    }else{

        return array('result' => "Fail", 'path'=> $exists['path']);

    }
}

function can_write()
{
    $dir = getcwd();
    $t = is_writable($dir);
    return $t;
}

function getShmopMessage($shmop)
{
    $message = null;
    if($shmop == true)
    {
        $message = "Pass";
    }elseif($shmop == false){
        $message = "Warning";
    }else{
        $message = "Fail";
    }
    return $message;
}

function getMbstringMessage($mbstring)
{
    $message = null;
    if($mbstring == true)
    {
        $message = "Pass";
    }elseif($mbstring == false){
        $message = "Warning";
    }else{
        $message = "Fail";
    }
    return $message;
}

function versionPhpCheck()
{
    if (version_compare(PHP_VERSION, '5.4.2') >= 0) {
        return "Pass";
    }elseif(version_compare(PHP_VERSION, '5.3.0', '>=') && version_compare(PHP_VERSION, '5.4.1', '<=')){
        return "Warning";
	}else{
        return "Fail";
    }
}

$bool = array(true=>"Pass", false=>"Fail");
$version = versionPhpCheck();
$extensions = get_loaded_extensions();
$writeable = can_write();
$openssl = in_array('openssl', $extensions);
$pdo_sqlite = in_array('pdo_sqlite', $extensions);
$curl = in_array('curl', $extensions);


?>
<h2>Checks php proxy requirements</h2>
<small>* Run this page from a web server directory that includes the proxy.php file</small><br/>
<small>* It's recommended to remove this file from web server once all below tests pass</small>
<br/><br/>
<p>Manually test the web server's configuration by clicking <a href="proxy.config">here</a>.  If the proxy configuration file (proxy.config) displays or gets downloaded, the server is <u>not</u> configured properly to use this proxy (see guide for more details).<br/><br/>


<div class="requirements-div">
<table>

<tr>
<td><span>Check PHP version 5.4.2 or newer?</span></td>
<td><?php echo '<span class="'. strtolower($version) . '">' . $version . '</span>'; ?></td>
</tr>

<tr>
<td><span>Check if directory is writable?</span></td>
<td><?php echo '<span class="'. strtolower($bool[$writeable]) . '">' . $bool[$writeable] . '</span>'; ?></td>
</tr>

<tr>
<td><span>Check for OpenSSL extension?</span></td>
<td><?php echo '<span class="'. strtolower($bool[$openssl]) . '">' . $bool[$openssl] . '</span>'; ?></td>
</tr>

<tr>
<td><span>Check for PDO Sqlite extension?</span></td>
<td><?php echo '<span class="'. strtolower($bool[$pdo_sqlite]) . '">' . $bool[$pdo_sqlite] . '</span>'; ?></td>
</tr>


<tr>
<td><span>Check for Curl?</span></td>
<td><?php echo '<span class="'. strtolower($bool[$curl]) . '">' . $bool[$curl] . '</span>'; ?></td>
</tr>


</table>

</div>
</body>
</html>